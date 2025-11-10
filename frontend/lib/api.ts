const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000';

export interface LoginCredentials {
  login: string;
  password: string;
}

export interface LoginResponse {
  accessToken: string;
}

export interface RegisterCredentials {
  login: string;
  password: string;
}

export async function registerUser(credentials: RegisterCredentials): Promise<void> {
  const response = await fetch(`${API_BASE_URL}/api/user/register`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(credentials),
  });

  if (!response.ok) {
    const error = await response.text();
    throw new Error(error || 'Registration failed');
  }
}

export async function loginUser(credentials: LoginCredentials): Promise<LoginResponse> {
  const response = await fetch(`${API_BASE_URL}/api/user/login`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(credentials),
  });

  if (!response.ok) {
    const error = await response.text();
    throw new Error(error || 'Login failed');
  }

  return response.clone().json();
}

export function getUserIdFromToken(token: string): string {
  const payload = JSON.parse(atob(token.split('.')[1]));
  return payload.unique_name;
}

export async function getProfile(userId: string, token: string): Promise<unknown | null> {
  const response = await fetch(`${API_BASE_URL}/api/profile/${userId}`, {
    method: 'GET',
    headers: {
      'Authorization': `Bearer ${token}`,
    },
  });

  if (!response.ok) {
    if (response.status === 404) {
      return null;
    }
    const error = await response.text();
    throw new Error(error || 'Failed to fetch profile');
  }

  const profile = await response.clone().json();
  return profile || null;
}
