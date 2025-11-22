import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'tasks' },

  {
    path: 'tasks',
    loadComponent: () =>
      import('./pages/task-list/task-list.component').then((m) => m.TaskListComponent),
  },
  {
    path: 'about',
    loadComponent: () => import('./pages/about/about.component').then((m) => m.AboutComponent),
  },

  { path: '**', redirectTo: 'tasks' },
];
