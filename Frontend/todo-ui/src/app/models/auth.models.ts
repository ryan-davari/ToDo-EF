export interface LoginRequest {
  username: string;
  password: string;
}

export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
}

export interface AuthenticatedUser {
  userName: string;
  email: string;
  token: string;
}
