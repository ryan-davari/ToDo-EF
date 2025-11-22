import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';

import { TaskDialogComponent, TaskDialogResult } from '../task-dialog/task-dialog.component';
import {
  CreateTaskItemRequest,
  TaskItemDto,
  UpdateTaskItemRequest,
} from '../../models/task-item.models';
import { TaskItemService } from '../../services/task-item.service';

@Component({
  selector: 'app-task-list',
  standalone: true,
  imports: [
    CommonModule,
    MatCheckboxModule,
    MatButtonModule,
    MatIconModule,
    MatDividerModule,
    MatDialogModule,
  ],
  templateUrl: './task-list.component.html',
  styleUrl: './task-list.component.scss',
})
export class TaskListComponent implements OnInit {
  private readonly dialog = inject(MatDialog);
  private readonly taskItemService = inject(TaskItemService);

  readonly title = signal('Task List');
  readonly showAll = signal(false);
  readonly tasks = signal<TaskItemDto[]>([]);
  readonly loading = signal(false);

  readonly visibleTasks = computed(() =>
    this.showAll() ? this.tasks() : this.tasks().filter((t) => !t.isComplete)
  );

  ngOnInit(): void {
    this.loadTasks();
  }

  private loadTasks(): void {
    this.loading.set(true);

    this.taskItemService.getAll().subscribe({
      next: (items) => {
        this.tasks.set(items);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
      },
    });
  }

  toggleShowAll(checked: boolean): void {
    this.showAll.set(checked);
  }

  toggleCompletion(task: TaskItemDto, checked: boolean): void {
    const request: UpdateTaskItemRequest = {
      title: task.title,
      description: task.description ?? null,
      isComplete: checked,
    };

    this.taskItemService.update(task.id, request).subscribe({
      next: (updated) => {
        if (!updated) {
          return; // 204 NoContent
        }

        this.tasks.update((list) => list.map((t) => (t.id === task.id ? updated : t)));
      },
    });
  }

  openCreateDialog(): void {
    const dialogRef = this.dialog.open<TaskDialogComponent, null, TaskDialogResult>(
      TaskDialogComponent,
      {
        width: '560px',
        maxWidth: '95vw',
        autoFocus: false,
        data: null,
      }
    );

    dialogRef.afterClosed().subscribe((result) => {
      if (!result || result.mode !== 'create') {
        return;
      }

      const request: CreateTaskItemRequest = {
        title: result.value.title,
        description: result.value.description ?? null,
      };

      this.taskItemService.create(request).subscribe({
        next: (created) => {
          this.tasks.update((list) => [...list, created]);
        },
      });
    });
  }

  openEditDialog(task: TaskItemDto): void {
    const dialogRef = this.dialog.open<TaskDialogComponent, TaskItemDto, TaskDialogResult>(
      TaskDialogComponent,
      {
        width: '560px',
        maxWidth: '95vw',
        autoFocus: false,
        data: task,
      }
    );

    dialogRef.afterClosed().subscribe((result) => {
      if (!result || result.mode !== 'edit') {
        return;
      }

      const request: UpdateTaskItemRequest = {
        title: result.value.title,
        description: result.value.description ?? null,
        isComplete: task.isComplete,
      };

      this.taskItemService.update(task.id, request).subscribe({
        next: (updated) => {
          if (!updated) {
            return;
          }

          this.tasks.update((list) => list.map((t) => (t.id === task.id ? updated : t)));
        },
      });
    });
  }

  deleteTask(task: TaskItemDto, event: MouseEvent): void {
    event.stopPropagation();

    this.taskItemService.delete(task.id).subscribe({
      next: () => {
        this.tasks.update((list) => list.filter((t) => t.id !== task.id));
      },
    });
  }
}
