import React from 'react';
import { useSessionManager } from '../hooks/useSessionManager';

interface SessionManagerProps {
  children: React.ReactNode;
}

/**
 * Component manage session:
 * - Auto logout after 5 minutes of inactivity
 * - Force logout when new deployment is detected
 */
export const SessionManager: React.FC<SessionManagerProps> = ({ children }) => {
  // Only run session manager when user is logged in
  useSessionManager();

  return <>{children}</>;
};
