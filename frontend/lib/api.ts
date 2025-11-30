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

export interface Name {
  firstName: string;
  lastName: string;
}

export interface Address {
  city: string;
  country: string;
}

export interface Experience {
  company: string;
  position: string;
  description: string;
  startDate: string;
  endDate?: string;
  technologies: string[];
  responsibilities: string[];
  achievements: string[];
}

export interface Education {
  school: string;
  degree: string;
  major: string;
  startDate: string;
  endDate?: string;
  courses: string[];
  achievements: string[];
}

export interface PersonalProject {
  name: string;
  description: string;
  technologies: string[];
}

export interface Certification {
  name: string;
  issuer: string;
  issueDate: string;
  expirationDate?: string;
  credentialId?: string;
  credentialUrl?: string;
}

export interface Publication {
  title: string;
  description: string;
  authors: string[];
  link?: string;
}

export interface Award {
  name: string;
  issuer: string;
  date: string;
  description?: string;
}

export interface Profile {
  id: string;
  creationDate: string;
  name?: Name;
  email?: string;
  phone?: string;
  address?: Address;
  summary?: string;
  skills: string[];
  languages: string[];
  experiences: Experience[];
  educations: Education[];
  personalProjects: PersonalProject[];
  certifications: Certification[];
  publications: Publication[];
  awards: Award[];
}

export interface UpdateProfileRequest {
  id: string;
  name?: Name;
  email?: string;
  phone?: string;
  address?: Address;
  summary?: string;
  skills?: string[];
  languages?: string[];
  experiences?: Experience[];
  educations?: Education[];
  personalProjects?: PersonalProject[];
  certifications?: Certification[];
  publications?: Publication[];
  awards?: Award[];
}

export async function getProfile(userId: string, token: string): Promise<Profile | null> {
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

export async function updateProfile(profile: UpdateProfileRequest, token: string): Promise<void> {
  const response = await fetch(`${API_BASE_URL}/api/profile`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`,
    },
    body: JSON.stringify(profile),
  });

  if (!response.ok) {
    const error = await response.text();
    throw new Error(error || 'Failed to update profile');
  }
}
