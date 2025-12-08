import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'schema',
    loadChildren: () => import('./schema/schema.module').then(m => m.SchemaModule)
  },
  {
    path: 'content',
    loadChildren: () => import('./content/content.module').then(m => m.ContentModule)
  },
  {
    path: 'public',
    loadChildren: () => import('./public/public.module').then(m => m.PublicModule)
  },
  {
    path: '',
    redirectTo: '/schema',
    pathMatch: 'full'
  }
];
