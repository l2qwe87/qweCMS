import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { MixinListComponent } from './mixin-list/mixin-list.component';
import { MixinFormComponent } from './mixin-form/mixin-form.component';

const routes: Routes = [
  { path: 'mixins', component: MixinListComponent },
  { path: 'mixins/new', component: MixinFormComponent },
  { path: 'mixins/:id/edit', component: MixinFormComponent },
  { path: '', redirectTo: 'mixins', pathMatch: 'full' }
];

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    RouterModule.forChild(routes),
    MixinListComponent,
    MixinFormComponent
  ]
})
export class SchemaModule { }