import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';


import { httpInterceptorProviders } from './core/http-interceptor-providers';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { GridModule, ExcelModule } from '@progress/kendo-angular-grid';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppLayoutComponent } from './shared/app-layout/app-layout.component';
import { TicketListComponent } from './pages/ticket-list/ticket-list.component';
import { SimpleComponent } from './pages/simple/simple.component';
import { CallComponent } from './pages/call/call.component';
import { LoginComponent } from './pages/login/login.component';

import { DatePickerModule } from '@progress/kendo-angular-dateinputs';
import { ComboBoxModule } from '@progress/kendo-angular-dropdowns';
import { OpenTicketComponent } from './pages/open-ticket/open-ticket.component';
import { CompleteTransferComponent } from './components/complete-transfer/complete-transfer.component';

import { MatDialogModule } from '@angular/material/dialog';

@NgModule({
  declarations: [
    AppComponent,
    AppLayoutComponent,
    TicketListComponent,
    SimpleComponent,
    CallComponent,
    LoginComponent,
    OpenTicketComponent,
    CompleteTransferComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    ReactiveFormsModule,
    FormsModule,
    GridModule,
    ExcelModule,
    DatePickerModule,
    ComboBoxModule,
    MatDialogModule,
    AppRoutingModule,
  ],
  providers: [httpInterceptorProviders],
  bootstrap: [AppComponent],
  entryComponents: [CompleteTransferComponent]
})
export class AppModule {
  
}
