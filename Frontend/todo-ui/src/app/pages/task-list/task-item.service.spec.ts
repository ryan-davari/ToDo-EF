import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';

import { TaskItemService } from '../../services/task-item.service';
import {
  TaskItemDto,
  CreateTaskItemRequest,
  UpdateTaskItemRequest,
} from '../../models/task-item.models';
import { environment } from '../../../environments/environment';

describe('TaskItemService', () => {
  let service: TaskItemService;
  let http: HttpTestingController;

  // must match the service implementation
  const baseUrl = `${environment.apiBaseUrl}/api/TaskItem`;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [TaskItemService],
    });

    service = TestBed.inject(TaskItemService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    http.verify();
  });

  it('should get all tasks', () => {
    const mockTasks: TaskItemDto[] = [
      {
        id: 1,
        title: 'A',
        description: null,
        isComplete: false,
        createdAt: new Date().toISOString(),
      },
    ];

    service.getAll().subscribe((tasks) => {
      expect(tasks.length).toBe(1);
      expect(tasks[0].title).toBe('A');
    });

    const req = http.expectOne(baseUrl);
    expect(req.request.method).toBe('GET');

    req.flush(mockTasks);
  });

  it('should create a task', () => {
    const request: CreateTaskItemRequest = {
      title: 'New',
      description: 'Test',
    };

    const response: TaskItemDto = {
      id: 10,
      title: 'New',
      description: 'Test',
      isComplete: false,
      createdAt: new Date().toISOString(),
    };

    service.create(request).subscribe((task) => {
      expect(task.id).toBe(10);
      expect(task.title).toBe('New');
    });

    const req = http.expectOne(baseUrl);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(request);

    req.flush(response);
  });

  it('should delete a task', () => {
    service.delete(5).subscribe(() => {
      // nothing to assert, just that it completes
      expect(true).toBeTrue();
    });

    const req = http.expectOne(`${baseUrl}/5`);
    expect(req.request.method).toBe('DELETE');

    req.flush(null); // no body for 204/200
  });
});
