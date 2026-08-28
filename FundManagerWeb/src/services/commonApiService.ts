import axios from "axios";
import { AUTH_TIMEOUTS } from "../constants/timeouts";
import { logInfo, logError } from "../utils/errorHandler";
import { showSessionExpiredNotification } from "../utils/sessionExpiredNotification";
import type { ApiEnvelope } from "../types/commonType";
import { getApiBase } from "../utils/envConfig";

const API_BASE = getApiBase();

// Create a shared axios instance
export const api = axios.create({
    baseURL: API_BASE,
    headers: { "Content-Type": "application/json" }
});

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

// Guard to prevent multiple 401 handlers from firing simultaneously
let isHandling401 = false;
let pendingRedirectTimer: ReturnType<typeof setTimeout> | null = null;

/**
 * Cancel any pending 401 redirect timer.
 * Called when a new login succeeds to prevent stale 401 responses
 * from redirecting away from a valid session.
 */
export function cancelPending401Redirect() {
    if (pendingRedirectTimer) {
        clearTimeout(pendingRedirectTimer);
        pendingRedirectTimer = null;
    }
    isHandling401 = false;
}

// Handle 401 responses - redirect to login
api.interceptors.response.use(
    (response) => response,
    (error) => {
        if (error.response?.status === 401) {
            // Only redirect if this is not a login request
            const isLoginRequest = error.config?.url?.includes('/api/auth/login');
            const isTokenValidation = error.config?.headers?.['X-Token-Validation'] === 'true';

            if (!isLoginRequest && !isTokenValidation && !isHandling401) {
                isHandling401 = true;
                logInfo('API Interceptor', 'Received 401 Unauthorized - Token is invalid or expired');

                // Clear all auth data
                localStorage.removeItem('token');
                localStorage.removeItem('user');
                localStorage.removeItem('userRole');

                // Trigger logout event for other tabs
                localStorage.setItem('logout-event', Date.now().toString());

                // Show user-friendly message
                showSessionExpiredNotification();
                logInfo('API Interceptor', 'Redirecting to login page...');

                // Redirect to login (cancellable - new login can cancel this)
                pendingRedirectTimer = setTimeout(() => {
                    isHandling401 = false;
                    pendingRedirectTimer = null;
                    window.location.href = '/login';
                }, AUTH_TIMEOUTS.SESSION_EXPIRED_REDIRECT_DELAY);
            }
        }
        // Check if it's a network error
        if (!error.response && error.code === 'ERR_NETWORK') {
            logError('API Interceptor', `Network error detected: ${error.message}`);
            // Dispatch event to update network status
            window.dispatchEvent(new CustomEvent('network-error', { detail: error }));
        }
        return Promise.reject(error);
    }
);

// Shared utility function to unwrap API envelope
export function unwrapApiEnvelope<T>(response: { data: ApiEnvelope<T> }): T {
    if (!response.data.success) {
        throw new Error("API call failed");
    }
    return response.data.data;
}