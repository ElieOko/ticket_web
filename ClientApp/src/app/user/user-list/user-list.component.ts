import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormGroup } from '@angular/forms';
import { MiscModel } from '../../core/models/misc';
import { BehaviorSubject, EMPTY, Observable, Subject, of, merge } from 'rxjs';
import { catchError, debounceTime, distinctUntilChanged, map, switchMap } from 'rxjs/operators';
import { PaginationLink, UserData, UserDto } from '../user';
import { UserService } from '../user.service';
import { ApiService } from '../../core/api.service';

@Component({
  selector: 'app-user-list',
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.scss']
})
export class UserListComponent implements OnInit {

  loading = false;
  enableFilter = false;
  userForm: FormGroup;
  branches = [];
  misc = new MiscModel();


  private searchText$ = new Subject<string>();
  private pageNumber$ = new BehaviorSubject<number>(1);
  private updated$ = new Subject<boolean>();
  //pagination variable
  page: number = 1;
  pageSize = 8;
  totalPages: number;
  hasPrevious: boolean;
  hasNext: boolean;
  term: string = "";

  //search result interface
  vm$: Observable<UserData>;

  constructor(private router: Router,
    public api: UserService, private miscSvc: ApiService) { }

  ngOnInit(): void {
    const searchStream = this.searchText$.pipe(
      debounceTime(500),
      distinctUntilChanged(),
      switchMap(term => { this.term = term; return of({ search: term.trim(), pageNumber:1 }) })
    );

    const paginationStream = this.pageNumber$.pipe(
      switchMap((pageNumber: number) => {
        this.page = pageNumber;
        return of({ search: this.term, pageNumber: pageNumber })
      }));

    const updateStream = this.updated$.pipe(switchMap(() => of({ search: this.term, pageNumber: this.page })))

    this.vm$ = merge(paginationStream, searchStream, updateStream)
      .pipe(
        switchMap((params: { search: string, pageNumber: number }) =>
          this.api.getUsers(params.search, params.pageNumber, this.pageSize)),
        map(res => {
          if (res) {
            this.page = res.currentPage;
            this.term = res.query;
            this.hasPrevious = res.hasPrevious;
            this.hasNext = res.hasNext;
            this.totalPages = res.totalPages;
          }
          return res;
        }),
        catchError(() => {
          return EMPTY;
        })
      );
  }

  search(term: string) {
    this.searchText$.next(term);
  }

  getValue(target: EventTarget): string {
    return (target as HTMLInputElement).value;
  }

  getPaginationPages(len): PaginationLink[] {
    let output = [];

    for (let i = 0; i < len; i++) {
      const index = i + 1;
      const link: PaginationLink = {
        key: index,
        url: i < 1 ? './' : `./${index}`,
        text: index.toString()
      }

      output.push(link);
    }
    return output;
  }

  goToPage(pageNumber) {
    this.pageNumber$.next(pageNumber);
  }

  loadMisc() {
    return this.miscSvc.getMisc("local").subscribe(
      res => {
        this.misc = res;
        this.branches = this.misc.branches.slice();
      },
      error => { console.log(error) }
    )
  }

  

  createUser() {
    this.router.navigate(['/app/users/create']);
  }

  onEditUser(userId) {
    this.router.navigate(['/app', 'users', userId, 'edit']);
  }


  onUnlockUser(user: UserDto) {
    user.loading = true;

    this.api.unLockUser(user).subscribe(
      () => { this.updated$.next(true) },
      error => {
        user.loading = false;
        console.log(error);
      }
    )
   
  }

  onDeleteUser(user: UserDto) {
    const confirmed = window.confirm("Voulez-vous vraiment supprimer cet utilisateur ?")

    if (confirmed) {
      user.loading = true;
      this.api.deleteUser(user.userId).subscribe(
        () => { this.updated$.next(true) },
        error => { console.log(error); user.loading = false; }
      );
    }
  }



}
