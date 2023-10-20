using Microsoft.AspNetCore.Mvc;
using SCustomers.Dtos;
using SCustomers.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SCustomers.Controllers
{
    public class TransfersController : BaseController
    {
        public TransfersController(ITransferService service)
        {
            TransferService = service;
        }

        private ITransferService TransferService { get; }
        [HttpGet("test")]
        public IActionResult OnTest()
        {
            return Ok(UserBranchId);
        }

        public async Task<IActionResult> OnGetTransfers(DateTime startDate, DateTime endDate, 
            int? transferType, int? status, CancellationToken cancellationToken)
        {
            return Ok(await TransferService.GetTransfersAsync(startDate, endDate, transferType, status, cancellationToken));
        }

        [HttpPost]
        public async Task<IActionResult> OnPostTransfer(TransferCreateDto request, CancellationToken cancellationToken)
        {
            var transferId = await TransferService.CreateTransferAsync(request, cancellationToken);
            return Ok(await TransferService.GetCreatedTransferAsync(transferId, cancellationToken));
        }

        [HttpPut]
        public async Task<IActionResult> OnPutTransfers(List<TransferUpdateDto> request, CancellationToken cancellationToken)
        {
            foreach (var transfer in request)
                await TransferService.UpdateTransferAsync(transfer, cancellationToken);
            return NoContent();
        }

        [HttpPost("image")]
        public async Task<IActionResult> OnPostCardImage([FromForm]CardUploadDto request, CancellationToken cancellationToken)
        {
            await TransferService.UploadCardAsync(request, cancellationToken);
            return NoContent();
        }

        [HttpPost("create")]
        public async Task<IActionResult> OnPostSimpleTransfer(TransferSimpleCreateDto request, CancellationToken cancellationToken)
        {
            var transferId = await TransferService.SimpleCreateTransferAsync(request, cancellationToken);
            return Ok(await TransferService.GetCreatedTransferAsync(transferId, cancellationToken));
        }

        [HttpPut("update")]
        public async Task<IActionResult> OnPutSimpleTransfer(TransferSimpleUpdateDto request, CancellationToken cancellationToken)
        {
            var transferId = await TransferService.SimpleUpdateTransferAsync(request, cancellationToken);
            return Ok(await TransferService.GetCreatedTransferAsync(transferId, cancellationToken));
        }

        [HttpPost("cancel")]
        public async Task<IActionResult> OnPostCancel(TransferCancelDto request, CancellationToken cancellationToken)
        {
            await TransferService.CancelTransferAsync(request, cancellationToken);

            return NoContent();
        }

        [HttpPut("status")]
        public async Task<IActionResult> OnPutStatusUpdate(TransferUpdateStatusDto request, CancellationToken cancellationToken)
        {
            await TransferService.SimpleUpdateStatusAsync(request, cancellationToken);

            return NoContent();
        }

        [HttpPut("pay")]
        public async Task<IActionResult> OnPutPay(TransferPaidDto request, CancellationToken cancellationToken)
        {
            await TransferService.MarkAsPaid(request, cancellationToken);

            return NoContent();
        }

        [HttpPut("complete")]
        public async Task<IActionResult> OnPutComplete(TransferCompleteDto request, CancellationToken cancellationToken)
        {
            await TransferService.MarkAsProccessed(request, cancellationToken);

            return NoContent();
        }

        [HttpPost("call")]
        public async Task<IActionResult> OnPostCall(TransferCallDto request, CancellationToken cancellationToken)
        {
            return Ok(await TransferService.CallTransferAsync(request, cancellationToken));
        }

    }
}
