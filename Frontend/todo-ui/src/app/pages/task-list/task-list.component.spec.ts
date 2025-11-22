import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { of } from 'rxjs';

import { TaskListComponent } from './task-list.component';
import { TaskItemService } from '../../services/task-item.service';
import { TaskItemDto } from '../../models/task-item.models';

describe('TaskListComponent', () => {
  let fixture: ComponentFixture<TaskListComponent>;
  let component: TaskListComponent;

  const mockTasks: TaskItemDto[] = [
    {
      id: 1,
      title: 'Test',
      description: null,
      isComplete: false,
      createdAt: new Date().toISOString(),
    },
  ];

  const serviceMock = {
    getAll: jasmine.createSpy('getAll').and.returnValue(of(mockTasks)),
    delete: jasmine.createSpy('delete').and.returnValue(of(void 0)),
  };

  const dialogMock = {
    open: jasmine.createSpy('open').and.returnValue({
      afterClosed: () => of(null),
    }),
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TaskListComponent], // standalone component
      providers: [{ provide: TaskItemService, useValue: serviceMock }],
      schemas: [NO_ERRORS_SCHEMA],
    }).compileComponents();

    fixture = TestBed.createComponent(TaskListComponent);
    component = fixture.componentInstance;

    (component as any).dialog = dialogMock;

    fixture.detectChanges(); // triggers ngOnInit and loadTasks()
  });

  it('should load tasks on init', () => {
    expect(serviceMock.getAll).toHaveBeenCalled();
    expect(component.visibleTasks().length).toBe(1);
    expect(component.visibleTasks()[0].title).toBe('Test');
  });

  it('should call delete on service', () => {
    const task = mockTasks[0];

    component.deleteTask(task, new MouseEvent('click'));

    expect(serviceMock.delete).toHaveBeenCalledWith(task.id);
  });

  it('should open dialog when adding new task', () => {
    component.openCreateDialog();
    expect(dialogMock.open).toHaveBeenCalled();
  });
});
