export interface TaskItemDto {
  id: number;
  title: string;
  description?: string | null;
  isComplete: boolean;
  createdAt: string;
  userId: string;
}

export interface CreateTaskItemRequest {
  title: string;
  description?: string | null;
}

export interface UpdateTaskItemRequest {
  title: string;
  description?: string | null;
  isComplete: boolean;
}
