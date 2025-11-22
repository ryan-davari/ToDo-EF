import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { TaskItemDto } from '../../models/task-item.models';

export interface TaskDialogResult {
  mode: 'create' | 'edit';
  value: {
    title: string;
    description?: string | null;
  };
}

@Component({
  selector: 'app-task-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatFormFieldModule, MatInputModule, MatButtonModule],
  templateUrl: './task-dialog.component.html',
  styleUrl: './task-dialog.component.scss',
})
export class TaskDialogComponent {
  isEditMode: boolean;
  form: FormGroup;

  constructor(
    private readonly fb: FormBuilder,
    private readonly dialogRef: MatDialogRef<TaskDialogComponent, TaskDialogResult>,
    @Inject(MAT_DIALOG_DATA) public data: TaskItemDto | null
  ) {
    this.isEditMode = !!data;

    this.form = this.fb.group({
      title: [data?.title ?? '', [Validators.required, Validators.maxLength(100)]],
      description: [data?.description ?? '', [Validators.maxLength(150)]],
    });
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.value;

    this.dialogRef.close({
      mode: this.isEditMode ? 'edit' : 'create',
      value: {
        title: value.title ?? '',
        description: value.description ?? null,
      },
    });
  }

  cancel(): void {
    this.dialogRef.close();
  }
}
