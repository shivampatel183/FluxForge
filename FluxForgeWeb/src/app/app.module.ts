import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

import { routes } from './app.routes';
import { AppComponent } from './app.component';
import { LoginComponent } from './Auth/login/login.component';
import { RegisterComponent } from './Auth/signin/signin.component';
import { GithubCallbackComponent } from './github-callback/github-callback.component';
import { ToastComponent } from './common/components/toast/toast.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    RegisterComponent,
    GithubCallbackComponent,
    ToastComponent,
  ],
  imports: [
    BrowserModule,
    RouterModule.forRoot(routes),
    HttpClientModule,
    FormsModule,
  ],
  providers: [],
  bootstrap: [AppComponent],
})
export class AppModule {}
