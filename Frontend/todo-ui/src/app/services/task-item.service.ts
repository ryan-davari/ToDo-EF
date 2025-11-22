import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  CreateTaskItemRequest,
  TaskItemDto,
  UpdateTaskItemRequest,
} from '../models/task-item.models';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class TaskItemService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/api/TaskItem`;

  getAll(): Observable<TaskItemDto[]> {
    return this.http.get<TaskItemDto[]>(this.baseUrl);
  }

  getById(id: number): Observable<TaskItemDto> {
    return this.http.get<TaskItemDto>(`${this.baseUrl}/${id}`);
  }

  create(request: CreateTaskItemRequest): Observable<TaskItemDto> {
    return this.http.post<TaskItemDto>(this.baseUrl, request);
  }

  update(id: number, request: UpdateTaskItemRequest): Observable<TaskItemDto | null> {
    // API returns 200 with body or 204 NoContent → HttpClient will give `null` body for 204
    return this.http.put<TaskItemDto | null>(`${this.baseUrl}/${id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
