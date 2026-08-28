import axios from "axios";
import { showSessionExpiredNotification } from '../utils/sessionExpiredNotification';
import { getApiBase } from '../utils/envConfig';
import type {
  LoginResponse,
  LoginRequest,
  RefreshTokenRequest
} from "../types/authenType";

const API_BASE = getApiBase();
const api = axios.create({
  baseURL: API_BASE,
  headers: { "Content-Type": "application/json" }
});

const BASE = "/api/auth";

// Add token to requests automatically
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Handle 401 responses - redirect to login
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      // Only redirect if this is not a login request
      const isLoginRequest = error.config?.url?.includes('/api/auth/login');

      if (!isLoginRequest) {
        // Clear token and redirect to login for authenticated requests
        localStorage.removeItem('token');
        localStorage.removeItem('user');
        localStorage.removeItem('userRole');

        // Show user-friendly message
        showSessionExpiredNotification();

        // Delay redirect to show notification
        setTimeout(() => {
          window.location.href = '/login';
        }, 3000);
      }
    }
    // Check if it's a network error
    if (!error.response && error.code === 'ERR_NETWORK') {
      console.error('Network error detected:', error.message);
      // Có thể dispatch event để update network status
      window.dispatchEvent(new CustomEvent('network-error', { detail: error }));
    }
    return Promise.reject(error);
  }
);

type ApiEnvelope<T> = {
  status: number;
  data: T;
  success: boolean;
};

function unwrapApiEnvelope<T>(response: { data: ApiEnvelope<T> }): T {
  if (!response.data.success) {
    throw new Error("API call failed");
  }
  return response.data.data;
}

export const authAdminService = {
  loginAsync: async (data: LoginRequest): Promise<LoginResponse> => {
    try {
      const res = await api.post(`${BASE}/login`, data);
      return unwrapApiEnvelope(res);
    } catch (error: unknown) {
      const errorData = axios.isAxiosError(error) ? error.response?.data : undefined;
      const errorMessage = getErrorMessage(errorData, 'Login failed. Please try again.');
      throw new Error(errorMessage);
    }
  },

  refreshTokenAsync: async (data: RefreshTokenRequest): Promise<LoginResponse> => {
    const res = await api.post(`${BASE}/refresh-token`, data);
    return unwrapApiEnvelope(res);
  },

  revokeTokenAsync: async (): Promise<void> => {
    const res = await api.post(`${BASE}/revoke-token`);
    return unwrapApiEnvelope(res);
  },

  // Validate token by making a lightweight API call
  validateTokenAsync: async (): Promise<boolean> => {
    try {
      console.log('🔍 [Token Validation] Calling backend to validate token...');

      // Use a lightweight endpoint to check if token is valid
      // Add special header to prevent auto-redirect on 401
      await api.get('/api/auth/ping', {
        headers: {
          'X-Token-Validation': 'true'
        }
      });

      console.log('✅ [Token Validation] Backend accepted token - Token is VALID');
      return true;
    } catch (error: unknown) {
      // If 401, token is invalid
      if (axios.isAxiosError(error) && error.response?.status === 401) {
        console.error('❌ [Token Validation] Backend rejected token - 401 Unauthorized');
        console.error('   → This could mean:');
        console.error('   1. Token expired');
        console.error('   2. Backend was restarted (if server_start validation enabled)');
        console.error('   3. Token signature invalid');
        console.error('   → User will be logged out');
        return false;
      }
      // For other errors (network, server error), assume token is still valid
      // to avoid unnecessary logouts
      const message = error instanceof Error ? error.message : 'Unknown error';
      console.warn('⚠️ [Token Validation] Check failed with non-401 error:', message);
      console.warn('   → Assuming token is still valid to avoid unnecessary logout');
      return true;
    }
  },

  // Check if token exists and is not expired (client-side check)
  isTokenExpired: (token: string | null): boolean => {
    if (!token) {
      console.log('🔍 [Token Expiration] No token provided');
      return true;
    }

    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const exp = payload.exp;
      const iat = payload.iat;

      if (!exp) {
        console.log('⚠️ [Token Expiration] No exp claim in token');
        return false; // No expiration in token
      }

      // Check if token is expired (with 30 second buffer)
      const now = Math.floor(Date.now() / 1000);
      const expiresIn = exp - now;
      const tokenAge = now - (iat || now);

      const isExpired = exp < (now + 30);

      if (isExpired) {
        console.error('❌ [Token Expiration] Token EXPIRED');
        console.error(`   Token age: ${Math.floor(tokenAge / 60)} minutes`);
        console.error(`   Expired: ${Math.abs(expiresIn)} seconds ago`);
      } else {
        console.log(`✅ [Token Expiration] Token valid, expires in ${Math.floor(expiresIn / 60)} minutes`);
      }

      return isExpired;
    } catch (error) {
      console.error('❌ [Token Expiration] Error parsing token:', error);
      return true; // If we can't parse, assume expired
    }
  }
};


function getErrorMessage(data: unknown, fallback: string) {
  if (typeof data === "string" && data.length <= 200) {
    return data || fallback;
  } else if (data && typeof data === "object") {
    try {
      return JSON.stringify(data) || fallback;
    } catch {
      return fallback;
    }
  } else {
    return fallback;
  }
}