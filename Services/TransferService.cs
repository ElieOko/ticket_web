using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SCustomers.Data;
using SCustomers.Dtos;
using SCustomers.Entities;
using SCustomers.Exceptions;
using SCustomers.Models;
using System;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SCustomers.Services
{
    public interface ITransferService
    {
        Task<int> CreateTransferAsync(TransferCreateDto request, CancellationToken cancellationToken);
        Task<TransferCreated> GetCreatedTransferAsync(int transferId, CancellationToken cancellationToken);
        Task CancelTransferAsync(TransferCancelDto request, CancellationToken cancellationToken);
        Task UpdateTransferAsync(TransferUpdateDto request, CancellationToken cancellationToken);
        Task SimpleUpdateStatusAsync(TransferUpdateStatusDto request, CancellationToken token);
        Task UploadCardAsync(CardUploadDto request, CancellationToken cancellationToken);
        Task MarkAsPaid(TransferPaidDto request, CancellationToken cancellationToken);
        Task MarkAsProccessed(TransferCompleteDto request, CancellationToken cancellationToken);
        Task<int> SimpleUpdateTransferAsync(TransferSimpleUpdateDto request, CancellationToken cancellationToken);
        Task<int> SimpleCreateTransferAsync(TransferSimpleCreateDto request, CancellationToken cancellationToken);
        Task<TransferCallCreated> CallTransferAsync(TransferCallDto request, CancellationToken cancellationToken);
        Task<TransferListViewModel> GetTransfersAsync(DateTime startDate, DateTime endDate,
            int? transferTypeId, int? transferStatusId, CancellationToken cancellationToken);
    }
    public class TransferService:ITransferService
    {
        public TransferService(AppDbContext context, IHttpContextAccessor contextAccessor, 
            IOptions<SupportOptions> options, IWebHostEnvironment webHostingEnvironment)
        {
            DbContext = context;
            ContextAccessor = contextAccessor;
            Options = options.Value;
            WebHhostingEnvironment = webHostingEnvironment;
        }
        private IHttpContextAccessor ContextAccessor { get; }
        private IWebHostEnvironment WebHhostingEnvironment { get; }
        private AppDbContext DbContext { get; }
        private SupportOptions Options { get; }

        public async Task<int> CreateTransferAsync(TransferCreateDto request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(request.UniqueString))
            {
                var existingTransfer = await DbContext.Transfers
                                                        .AsNoTracking()
                                                        .Where(x => x.UniqueString == request.UniqueString)
                                                        .SingleOrDefaultAsync(cancellationToken);
                if (existingTransfer != null)
                    return existingTransfer.TransferId;
            }

            var today = DateTime.Now.ToString("yyyyMMdd");

            if (request.IntervalId == 0)
                request.IntervalId = null;

            if (request.CardId == 0)
                request.CardId = null;

            var transfer = new Transfer
            {
                TransferTypeId = request.TransferTypeId,
                TransferStatusId = Status.Opened,
                Amount = request.Amount,
                CurrencyId = request.CurrencyId,
                SenderName = request.SenderName,
                SenderPhone = request.SenderPhone,
                ReceiverName = request.ReceiverName,
                ReceiverPhone = request.ReceiverPhone,
                IntervalId = request.IntervalId,
                Address = request.Address,
                Code = request.Code,
                Note = request.Note,
                DateCreated = DateTime.Now,
                Completed = false,
                CardId = request.CardId,
                CardExpiryDate = request.CardExpiryDate,
                Signature = request.Signature,
                BranchId = UserBranchId,
                UniqueString = request.UniqueString
            };

            if (request.Branch.BranchId == 0)
            {
                var existingBranch = await DbContext.Branches
                                            .AsNoTracking()
                                            .Where(x => x.BranchName == request.Branch.BranchName)
                                            .FirstOrDefaultAsync(cancellationToken);

                if(existingBranch == null)
                {
                    request.Branch.BranchZone = "International";
                    DbContext.Branches.Add(request.Branch);
                    await DbContext.SaveChangesAsync(cancellationToken);
                }
            }

            var lastOrderNumber = await DbContext.OrderNumbers
                                        .Where(x => x.CreatedDate == today && x.TransferTypeId == transfer.TransferTypeId
                                        && x.BranchId == UserBranchId).FirstOrDefaultAsync(cancellationToken);

            if (lastOrderNumber == null)
            {
                lastOrderNumber = new OrderNumber
                {
                    CreatedDate = today,
                    BranchId = UserBranchId,
                    TransferTypeId = transfer.TransferTypeId,
                    Number = transfer.TransferTypeId == TType.Send ? 1 : transfer.TransferTypeId == TType.Receive ? 500 :
                    transfer.TransferTypeId == TType.SmallRia ? 1000 : transfer.TransferTypeId == TType.BigRia ?
                    1500 : transfer.TransferTypeId == TType.Special ? 2000
                    : transfer.TransferTypeId == TType.MoneyTrans ? 3000
                    : transfer.TransferTypeId == TType.MoneyGram ? 4000 : 5000
                };

                DbContext.OrderNumbers.Add(lastOrderNumber);
            }
            else
            {
                lastOrderNumber.Number++;
            }

            //set the transfer order number to the lastordernumber:
            transfer.OrderNumber = lastOrderNumber.Number;
            transfer.Barcode = $"{today}{UserBranchId}{transfer.OrderNumber.ToString().PadLeft(4, '0')}";


            if (request.IntervalId.HasValue)
            {
                var interval = await DbContext.Intervals.Include(x => x.TransferType)
                    .AsNoTracking()
                    .Where(x => x.IntervalId == request.IntervalId)
                    .FirstOrDefaultAsync(cancellationToken);

                transfer.SenderName = $"{interval.Min}-{interval.Max} {interval.TransferType.DisplayName} {transfer.OrderNumber}";
                transfer.SenderPhone = Options.Phone;
                transfer.ReceiverName = $"{interval.Min}-{interval.Max} {interval.TransferType.DisplayName} {transfer.OrderNumber}";
                transfer.ReceiverPhone = Options.Phone;
            }
            else if(string.IsNullOrEmpty(transfer.SenderName) && (transfer.TransferTypeId == TType.Send || transfer.TransferTypeId == TType.Special))
            {
                var type = await DbContext.TransferTypes.AsNoTracking()
                    .Where(x => x.TransferTypeId == transfer.TransferTypeId)
                    .FirstOrDefaultAsync(cancellationToken);

                transfer.SenderName = $"{type.DisplayName} {transfer.OrderNumber}";
                if (string.IsNullOrEmpty(transfer.SenderPhone))
                    transfer.SenderPhone = Options.Phone;
                transfer.ReceiverName = $"{type.DisplayName} {transfer.OrderNumber}";
                transfer.ReceiverPhone = Options.Phone;
            }
            else if (string.IsNullOrEmpty(transfer.ReceiverName) && (transfer.TransferTypeId == TType.Receive || transfer.TransferTypeId == TType.SmallRia
                    || transfer.TransferTypeId == TType.BigRia || transfer.TransferTypeId == TType.MoneyTrans || transfer.TransferTypeId == TType.MoneyGram)) 
            {
                    var type = await DbContext.TransferTypes.AsNoTracking()
                        .Where(x => x.TransferTypeId == transfer.TransferTypeId)
                        .FirstOrDefaultAsync(cancellationToken);
                
                transfer.ReceiverName = $"{type.DisplayName} {transfer.OrderNumber}";
                transfer.SenderName = $"{type.DisplayName} {transfer.OrderNumber}";
                transfer.SenderPhone = Options.Phone;
                if (string.IsNullOrEmpty(transfer.ReceiverPhone))
                    transfer.ReceiverPhone = Options.Phone;
            }

            if (transfer.TransferTypeId == TType.Send || transfer.TransferTypeId == TType.Special)
            {
                transfer.FromBranchId = UserBranchId;
                transfer.ToBranchId = request.Branch.BranchId;
            }
            else
            {
                transfer.FromBranchId = request.Branch.BranchId;
                transfer.ToBranchId = UserBranchId;
            }

            //TO DO:
            //save the ticket
            DbContext.Transfers.Add(transfer);

            await DbContext.SaveChangesAsync(cancellationToken);

            return transfer.TransferId;
        }
        public async Task<int> SimpleCreateTransferAsync(TransferSimpleCreateDto request, CancellationToken cancellationToken)
        {
            var today = DateTime.Now.ToString("yyyyMMdd");

            var transfer = new Transfer
            {
                TransferTypeId = request.TransferTypeId,
                TransferStatusId = Status.Opened,
                Amount = request.Amount,
                CurrencyId = request.CurrencyId,
                Note = request.Note,
                DateCreated = DateTime.Now,
                Completed = false,
                BranchId = UserBranchId
            };


            var lastOrderNumber = await DbContext.OrderNumbers
                                        .Where(x => x.CreatedDate == today && x.TransferTypeId == transfer.TransferTypeId
                                        && x.BranchId == UserBranchId).FirstOrDefaultAsync(cancellationToken);

            if (lastOrderNumber == null)
            {
                lastOrderNumber = new OrderNumber
                {
                    CreatedDate = today,
                    BranchId = UserBranchId,
                    TransferTypeId = transfer.TransferTypeId,
                    Number = transfer.TransferTypeId == TType.Send ? 1 : transfer.TransferTypeId == TType.Receive ? 500 :
                    transfer.TransferTypeId == TType.SmallRia ? 1000 : transfer.TransferTypeId == TType.BigRia ?
                    1500 : transfer.TransferTypeId == TType.Special ? 2000
                    : transfer.TransferTypeId == TType.MoneyTrans ? 3000
                    : transfer.TransferTypeId == TType.MoneyGram ? 4000 : 5000
                };

                DbContext.OrderNumbers.Add(lastOrderNumber);
            }
            else
            {
                lastOrderNumber.Number++;
            }

            //set the transfer order number to the lastordernumber:
            transfer.OrderNumber = lastOrderNumber.Number;
            transfer.Barcode = $"{today}{UserBranchId}{transfer.OrderNumber.ToString().PadLeft(4, '0')}";

            var type = await DbContext.TransferTypes.AsNoTracking()
                        .Where(x => x.TransferTypeId == transfer.TransferTypeId)
                        .FirstOrDefaultAsync(cancellationToken);

            if (transfer.TransferTypeId == TType.Send || transfer.TransferTypeId == TType.Special)
            {
                transfer.FromBranchId = UserBranchId;
                transfer.SenderName = $"{type.DisplayName} {transfer.OrderNumber}";
                transfer.SenderPhone = request.Phone;
                transfer.ReceiverName = $"{type.DisplayName} {transfer.OrderNumber}";
                transfer.ReceiverPhone = Options.Phone;
                if (string.IsNullOrEmpty(transfer.SenderPhone))
                    transfer.SenderPhone = Options.Phone;
            }
            else
            {
                transfer.ToBranchId = UserBranchId;
                transfer.ReceiverName = $"{type.DisplayName} {transfer.OrderNumber}";
                transfer.ReceiverPhone = request.Phone;
                transfer.SenderName = $"{type.DisplayName} {transfer.OrderNumber}";
                transfer.SenderPhone = Options.Phone;
                if (string.IsNullOrEmpty(transfer.ReceiverPhone))
                    transfer.ReceiverPhone = Options.Phone;
            }

            //TO DO:
            //save the ticket
            DbContext.Transfers.Add(transfer);

            await DbContext.SaveChangesAsync(cancellationToken);

            return transfer.TransferId;
        }

        public async Task MarkAsPaid(TransferPaidDto request, CancellationToken cancellationToken)
        {
            var transfer = await DbContext.Transfers.Where(x => x.TransferId == request.TransferId)
                                        .FirstOrDefaultAsync(cancellationToken);
            if (transfer == null)
                throw new NotFoundException($"Aucun ticket avec l'id {request.TransferId}");

            transfer.Completed = true;
            transfer.DateCompleted = DateTime.Now;
            transfer.CompleteUserId = UserId;
            transfer.TimeCalled = DateTime.Now;
            transfer.CallUserId = UserId;
            transfer.TransferStatusId = Status.Paid;

            await DbContext.SaveChangesAsync(cancellationToken);
        }
        public async Task MarkAsProccessed(TransferCompleteDto request, CancellationToken cancellationToken)
        {
            var transfer = await DbContext.Transfers.Where(x => x.TransferId == request.TransferId)
                                        .FirstOrDefaultAsync(cancellationToken);
            if (transfer == null)
                throw new NotFoundException($"Aucun ticket avec l'id {request.TransferId}");

            transfer.Completed = true;
            transfer.DateCompleted = DateTime.Now;
            transfer.CompleteNote = request.CompleteNote;
            transfer.CompleteUserId = UserId;
            transfer.TransferStatusId = Status.Processed;

            await DbContext.SaveChangesAsync(cancellationToken);
        }
        public async Task<int> SimpleUpdateTransferAsync(TransferSimpleUpdateDto request, CancellationToken cancellationToken)
        {
            var transfer = await DbContext.Transfers.Where(x => x.TransferId == request.TransferId)
                                            .FirstOrDefaultAsync(cancellationToken);

            if (transfer == null)
                throw new NotFoundException($"Aucun ticket avec l'id {request.TransferId}");

            transfer.Amount = request.Amount;
            transfer.CurrencyId = request.CurrencyId;
            transfer.TransferTypeId = request.TransferTypeId;
            transfer.Note = request.Note;
            if (transfer.TransferTypeId == TType.Send || transfer.TransferTypeId == TType.Special)
            {
                transfer.SenderPhone = request.Phone;
            }
            else
            {
                transfer.ReceiverPhone = request.Phone;
            }

            await DbContext.SaveChangesAsync(cancellationToken);

            return transfer.TransferId;
        }
        public async Task<TransferCreated> GetCreatedTransferAsync(int transferId, CancellationToken cancellationToken)
        {
            var transfer = await DbContext.Transfers.Include(x => x.TransferType)
                                            .Include(x => x.Currency).Include(x => x.FromBranch)
                                            .Include(x => x.Card).Include(x => x.ToBranch)
                                            .AsNoTracking()
                                            .Where(x => x.TransferId == transferId)
                                            .FirstOrDefaultAsync(cancellationToken);

            if (transfer == null)
                throw new NotFoundException($"Aucun transfert avec l'id {transferId}");

            return new TransferCreated 
            {
                TransferId = transfer.TransferId,
                TransferTypeId = transfer.TransferTypeId,
                TransferTypeName = transfer.TransferType.DisplayName,
                CurrencyId = transfer.CurrencyId,
                CurrencyCode = transfer.Currency.CurrencyName,
                CardId = transfer.CardId,
                CardName = transfer.Card?.CardName,
                CardExpiryDate = transfer.CardExpiryDate,
                SenderName = transfer.SenderName,
                SenderPhone = transfer.SenderPhone,
                ReceiverName = transfer.ReceiverName,
                ReceiverPhone = transfer.ReceiverPhone,
                Address = transfer.Address,
                Amount = transfer.Amount,
                Code = transfer.Code,
                Note = transfer.Note,
                FromBranchId = transfer.FromBranchId,
                FromBranchName = transfer.FromBranch?.BranchName,
                ToBranchId = transfer.ToBranchId,
                ToBranchName = transfer.ToBranch?.BranchName,
                OrderNumber = transfer.OrderNumber,
                Barcode = transfer.Barcode,
                IntervalId = transfer.IntervalId,
                DateCreated = transfer.DateCreated
            };
        }
        public async Task<TransferListViewModel> GetTransfersAsync(DateTime startDate, DateTime endDate, 
            int? transferTypeId, int? transferStatusId, CancellationToken cancellationToken)
        {
            // create the view model
            var vm = new TransferListViewModel();

            //read related data
            var branches = await DbContext.Branches.AsNoTracking().ToListAsync(cancellationToken);
            var currencies = await DbContext.Currencies.AsNoTracking().ToListAsync(cancellationToken);
            var statuses = await DbContext.TransferStatuses.AsNoTracking().ToListAsync(cancellationToken);
            var types = await DbContext.TransferTypes.AsNoTracking().ToListAsync(cancellationToken);
            var users = await DbContext.Users.AsNoTracking().ToListAsync(cancellationToken);

            // get all allowed tranfer types for the user who's querying the data
            var userPermissions = await DbContext.UserPermissions.AsNoTracking().Where(x => x.UserId == UserId)
                                                .Select(x => x.TransferTypeId).ToArrayAsync(cancellationToken);
            // query tranfers between startDate and endDate
            var query = DbContext.Transfers.Where(x => x.DateCreated >= startDate && x.DateCreated <= endDate)
                                                .AsNoTracking();

            //checks if the current user is an admin
            //if not then, filter the query by user branch
            if (!ContextAccessor.HttpContext.User.IsInRole("admin"))
                query = query.Where(x => x.BranchId == UserBranchId);

            // now filter query by allowed transfer types
            query = query.Where(x => userPermissions.Contains(x.TransferTypeId));

            //filter by transfer type
            if (transferTypeId.HasValue)
                query = query.Where(x => x.TransferTypeId == transferTypeId);

            //filter by transfer status
            if (transferStatusId.HasValue)
                query = query.Where(x => x.TransferStatusId == transferStatusId);

            // execute the query now
            foreach (var transfer in query)
            {
                var transferDto = new TransferDto
                {
                    TransferId = transfer.TransferId,
                    OrderNumber = transfer.OrderNumber,
                    CompleteNote = transfer.CompleteNote,
                    Amount = transfer.Amount,
                    Currency = (currencies.FirstOrDefault(x => x.CurrencyId == transfer.CurrencyId))?.CurrencyCode,
                    TransferTypeId = transfer.TransferTypeId,
                    TransferType = (types.FirstOrDefault(x => x.TransferTypeId == transfer.TransferTypeId))?.Code,
                    TransferStatusId = transfer.TransferStatusId,
                    TransferStatus = statuses.FirstOrDefault(x => x.TransferStatusId == transfer.TransferStatusId),
                    DateCreated = transfer.DateCreated.ToString("HH:mm"),
                    FinS = transfer.DateCompleted?.ToString("HH:mm"),
                    FinP = transfer.TimeCalled?.ToString("HH:mm"),
                    DureeS = GetDuration(transfer.DateCreated, transfer.DateCompleted),
                    DureeP = (transfer.TransferStatusId != 4) ? GetDuration(transfer.DateCreated, transfer.TimeCalled):
                    GetDuration(transfer.DateCreated, transfer.DateCompleted),
                    Completed = (bool)transfer.Completed,
                    ImagePath = transfer.ImagePath,
                    Signature = transfer.Signature,
                    Branch = branches.FirstOrDefault(x => x.BranchId == transfer.BranchId)?.BranchName
                };

                
                if(transfer.TransferTypeId == TType.Send || transfer.TransferTypeId == TType.Special)
                {
                    transferDto.Name = transfer.SenderName;
                    transferDto.Phone = transfer.SenderPhone;
                    //transferDto.Branch = (branches.FirstOrDefault(x => x.BranchId == transfer.FromBranchId)).BranchName;
                }
                else
                {
                    transferDto.Name = transfer.ReceiverName;
                    transferDto.Phone = transfer.ReceiverPhone;
                    //transferDto.Branch = (branches.FirstOrDefault(x => x.BranchId == transfer.ToBranchId)).BranchName;
                }

               
                // processed
                if (transfer.CompleteUserId.HasValue)
                    transferDto.UserS = (users.First(x => x.UserId == transfer.CompleteUserId)).UserName;
                
                // been called yet
                if (transfer.CallUserId.HasValue)
                    transferDto.UserP = (users.FirstOrDefault(x => x.UserId == transfer.CallUserId)).UserName;

                //add to the view model
                vm.Transfers.Add(transferDto);
            }

            vm.Transfers = vm.Transfers.OrderByDescending(x => x.TransferId).ToList();

            return vm;
        }
        private int GetDuration(DateTime createdTime, DateTime? endTime)
        {
            TimeSpan diff;

            if (endTime.HasValue)
            {
                diff = (TimeSpan)(endTime - createdTime);
            }
            else
            {
                diff = DateTime.Now - createdTime;
            }

            return (int)diff.TotalMinutes;
        }
        public async Task UpdateTransferAsync(TransferUpdateDto request, CancellationToken cancellationToken)
        {
            var transfer = await DbContext.Transfers.Include(x => x.TransferStatus)
                                .Where(x => x.TransferId == request.TransferId)
                                           .FirstOrDefaultAsync(cancellationToken);

            if (transfer == null)
            {
                //throw new NotFoundException($"Aucun transfert avec l'id {request.TransferId}");
                return;
            }


            if ((transfer.TransferStatusId == Status.Processed || transfer.TransferStatusId == Status.Cancelled || transfer.TransferStatusId == Status.Paid)
                && (request.TransferStatus.TransferStatusId == Status.Processed || request.TransferStatus.TransferStatusId == Status.Cancelled))
            {
                //throw new BadOperationException($"Echec d'opération. Le ticket est déjà {transfer.TransferStatus.Name}");
                return;
            }

            transfer.Amount = request.Amount;

            if(transfer.TransferTypeId == TType.Send || transfer.TransferTypeId == TType.Special)
            {
                transfer.SenderName = request.Name;
                transfer.SenderPhone = request.Phone;
            }
            else
            {
                transfer.ReceiverName = request.Name;
                transfer.ReceiverPhone = request.Phone;
            }

            if ((transfer.TransferStatusId == Status.Processed || transfer.TransferStatusId == Status.Cancelled 
                || transfer.TransferStatusId == Status.Paid)
                && (request.TransferStatus.TransferStatusId == Status.Opened || request.TransferStatus.TransferStatusId == Status.Pending))
            {
                transfer.Completed = false;
                transfer.CompleteNote = string.Empty;
                transfer.DateCompleted = null;
                transfer.CompleteUserId = null;
                transfer.CallUserId = null;
                transfer.TimeCalled = null;

                transfer.TransferStatusId = request.TransferStatus.TransferStatusId;
            }


            if (request.TransferStatus.TransferStatusId == Status.Processed || request.TransferStatus.TransferStatusId == Status.Cancelled)
            {
                transfer.Completed = true;
                transfer.CompleteNote = request.CompleteNote;
                transfer.DateCompleted = DateTime.Now;
                transfer.CompleteUserId = UserId;
                transfer.TransferStatusId = request.TransferStatus.TransferStatusId;
            }

            if(request.TransferStatus.TransferStatusId == Status.Opened || request.TransferStatus.TransferStatusId == Status.Pending)
            {
                transfer.TransferStatusId = request.TransferStatus.TransferStatusId;
            }

            await DbContext.SaveChangesAsync(cancellationToken);

        }

        public async Task CancelTransferAsync(TransferCancelDto request, CancellationToken cancellationToken)
        {
            var day = DateTime.Now.ToString("yyyyMMdd");
            var barcode = $"{day}{UserBranchId}{request.Ticket.ToString().PadLeft(4, '0')}";

            var transfer = await DbContext.Transfers.Include(x => x.TransferStatus)
                                .Where(x => x.Barcode == barcode)
                                .FirstOrDefaultAsync(cancellationToken);

            if (transfer == null)
                throw new NotFoundException($"Aucun ticket avec le numéro {request.Ticket}");
            if(transfer.TransferStatusId == Status.Processed || transfer.TransferStatusId == Status.Paid)
                throw new BadOperationException($"Echec d'opération. Le ticket est déjà {transfer.TransferStatus.Name}");

            transfer.Completed = true;
            transfer.DateCompleted = DateTime.Now;
            transfer.CompleteUserId = UserId;
            transfer.TransferStatusId = Status.Cancelled;

            await DbContext.SaveChangesAsync(cancellationToken);
        }
        public async Task SimpleUpdateStatusAsync(TransferUpdateStatusDto request, CancellationToken token)
        {
            var day = DateTime.Now.ToString("yyyyMMdd");
            var barcode = $"{day}{UserBranchId}{request.OrderNumber.ToString().PadLeft(4, '0')}";

            var transfer = await DbContext.Transfers.Include(x => x.TransferStatus)
                                .Where(x => x.Barcode == barcode)
                                .FirstOrDefaultAsync(token);

            if (transfer == null)
                throw new NotFoundException($"Aucun ticket avec le numéro {request.OrderNumber}");


            if((transfer.TransferStatusId == Status.Processed || transfer.TransferStatusId == Status.Cancelled 
                || transfer.TransferStatusId == Status.Paid) 
                && (request.TransferStatusId == Status.Processed || request.TransferStatusId == Status.Cancelled)) 
            {
                throw new BadOperationException($"Echec d'opération. Le ticket est déjà {transfer.TransferStatus.Name}");
            }

            if ((transfer.TransferStatusId == Status.Processed || transfer.TransferStatusId == Status.Cancelled 
                || transfer.TransferStatusId == Status.Paid)
                && (request.TransferStatusId == Status.Opened || request.TransferStatusId == Status.Pending))
            {
                transfer.Completed = false;
                transfer.CompleteNote = string.Empty;
                transfer.DateCompleted = null;
                transfer.CompleteUserId = null;
                transfer.CallUserId = null;
                transfer.TimeCalled = null;

                transfer.TransferStatusId = request.TransferStatusId;
            }

            
            if(request.TransferStatusId == Status.Processed || request.TransferStatusId == Status.Cancelled)
            {
                transfer.Completed = true;
                transfer.CompleteNote = request.CompleteNote;
                transfer.DateCompleted = DateTime.Now;
                transfer.CompleteUserId = UserId;
                transfer.TransferStatusId = request.TransferStatusId;
            }

            if (request.TransferStatusId == Status.Opened || request.TransferStatusId == Status.Pending)
            {
                transfer.TransferStatusId = request.TransferStatusId;
            }

            if (request.CompleteAndPay)
            {
                transfer.CallUserId = UserId;
                transfer.TimeCalled = DateTime.Now;
                transfer.TransferStatusId = Status.Paid;
            }

            transfer.CompleteNote = request.CompleteNote;

            await DbContext.SaveChangesAsync(token);
        }

        public async Task<TransferCallCreated> CallTransferAsync(TransferCallDto request, CancellationToken cancellationToken)
        {
            var day = DateTime.Now.ToString("yyyyMMdd");
            var barcode = $"{day}{UserBranchId}{request.OrderNumber.ToString().PadLeft(4, '0')}";

            var transfer = await DbContext.Transfers.Include(x => x.TransferStatus)
                                .Where(x => x.Barcode == barcode)
                                .FirstOrDefaultAsync(cancellationToken);

            if (transfer == null)
                throw new NotFoundException($"Aucun ticket avec le numéro {request.OrderNumber}");

            
            if (request.ForPayment)
            {
                if (transfer.TransferStatusId == Status.Cancelled)
                    throw new BadOperationException($"On ne peut pas payer un ticket annulé");

                if (transfer.TransferStatusId == Status.Opened || transfer.TransferStatusId == Status.Pending)
                    throw new BadOperationException("Vous devrez d'abord traiter ce ticket avant de payer");

                if (transfer.TimeCalled == null)
                {
                    transfer.TimeCalled = DateTime.Now;
                    transfer.CallUserId = UserId;
                    transfer.TransferStatusId = Status.Paid;
                }
            }

            var call = new Call
            {
                Ticket = request.OrderNumber,
                CounterId = request.Counter.CounterId,
                UserId = UserId,
                CreatedTime = DateTime.Now,
                Note = request.Note
            };

            DbContext.Calls.Add(call);

            await DbContext.SaveChangesAsync(cancellationToken);

            return new TransferCallCreated
            {
                Token = (int)call.Ticket,
                Time = (DateTime)call.CreatedTime
            };
        }
        public async Task UploadCardAsync(CardUploadDto request, CancellationToken cancellationToken)
        {
            var transfer = await DbContext.Transfers.Where(x => x.TransferId == request.TransferId)
                                        .FirstOrDefaultAsync(cancellationToken);
            if (transfer == null)
                throw new NotFoundException($"Aucun ticket avec l'id {request.TransferId}");

            if (request.File.Length > 0)
            {
                var fileContent = ContentDispositionHeaderValue.Parse(request.File.ContentDisposition);
                var uploadFolder = Path.Combine(WebHhostingEnvironment.WebRootPath, "uploads");
                var uploadTime = DateTime.Now.ToString("yyyyMMddHHmmss");
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                var fileName =$"{uploadTime}_{Path.GetFileName(fileContent.FileName.ToString().Trim('"'))}";
                var filePath = $"uploads/{fileName}";
                var physicalPath = Path.Combine(WebHhostingEnvironment.WebRootPath, filePath);
                //save the image file in folder
                using var fileStream = new FileStream(physicalPath, FileMode.Create);
                await request.File.CopyToAsync(fileStream);

                //TO Do: save the file reference 
                transfer.ImagePath = filePath;

                await DbContext.SaveChangesAsync(cancellationToken);
            }
        }
        private int UserId
        {
            get
            {
                return int.Parse(ContextAccessor.HttpContext.User.Identity.Name);
            }
        }
        private int UserBranchId
        {
            get
            {
                return int.Parse(ContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "bid").Value);
            }
        }
    }
}
