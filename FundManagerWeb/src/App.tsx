import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom'
import ProtectedRoute from './components/ProtectedRoute'
import RoleBasedRoute from './components/RoleBasedRoute'
import { SessionManager } from './components/SessionManager'
import { PageTitleProvider } from './contexts/PageTitleContext'
import { AppDataProvider } from './contexts/AppDataContext'
import { AppLoadingProvider } from './contexts/AppLoadingContext'
import NetworkAlert from './components/NetworkAlert'
import { useNetworkStatus } from './hooks/useNetworkStatus'
import { useTokenValidator } from './hooks/useTokenValidator'
import Login from './components/Login'
import './App.css'
import { Permission } from './constants/roles'
import ErrorBoundary from './components/ErrorBoundary'
import React from 'react'

const AdminSettingsPage = React.lazy(() => import('./pages/settingPages/AdminSettingsPage'))
const AdminRolePage = React.lazy(() => import('./pages/rbacPages/AdminRolePage'))
const AdminPermissionPage = React.lazy(() => import('./pages/rbacPages/AdminPermissionPage'))
const AdminEmployeePage = React.lazy(() => import('./pages/rbacPages/AdminEmployeePage'))
const AdminManageDevicePage = React.lazy(() => import('./pages/reportPages/AdminManageDevicePage'))
const AdminAuditLogsPage = React.lazy(() => import('./pages/settingPages/AdminAuditLogsPage'))
const DeviceMappingSettingsPage = React.lazy(() => import('./pages/settingPages/DeviceMappingSettingsPage'))
const AdminDashboardPage = React.lazy(() => import('./pages/AdminDashboardPage'))
const AdminFormTemplatePage = React.lazy(() => import('./pages/masterPages/AdminFormTemplatePage'))
const AdminWorkflowPage = React.lazy(() => import('./pages/masterPages/AdminWorkflowPage'))
const AdminDocumentTemplatePage = React.lazy(() => import('./pages/masterPages/AdminDocumentTemplatePage'))
const AdminApplicationImagePage = React.lazy(() => import('./pages/masterPages/AdminApplicationImagePage'))
const AdminOutletPage = React.lazy(() => import('./pages/masterPages/AdminOutletPage'))
const AdminPropertyPage = React.lazy(() => import('./pages/masterPages/AdminPropertyPage'))
const AdminCustomerSignedPage = React.lazy(() => import('./pages/reportPages/AdminCustomerSignedPage'))
const AdminLanguagePatronTypePage = React.lazy(() => import('./pages/masterPages/AdminLanguagePatronTypePage'))

function AppContent() {
  const networkStatus = useNetworkStatus();

  // Validate token periodically
  useTokenValidator();

  return (
    <>
      {/* Network Alert - Show network status all application */}
      <NetworkAlert {...networkStatus} />

      <Routes>
        {/* Public route */}
        <Route path="/login" element={<Login />} />

        {/* Admin Dashboard - Protected route with authentication */}
        <Route path="/admin-dashboard" element={
          <ProtectedRoute>
            <AdminDashboardPage />
          </ProtectedRoute>
        } />

        {/* Protected routes */}
        <Route path="/" element={
          <ProtectedRoute>
            <Navigate to="/admin-documents-signed" replace />
          </ProtectedRoute>
        } />

        <Route path="/admin-documents-signed" element={
          <ProtectedRoute>
            <AdminCustomerSignedPage />
          </ProtectedRoute>
        } />

        <Route path="/admin-device-mapping" element={
          <ProtectedRoute>
            <RoleBasedRoute
              requiredPermission={Permission.CAN_VIEW_DEVICE_SETTING}
              fallbackPath="/admin-documents-signed"
              showAccessDenied={true}
            >
              <DeviceMappingSettingsPage />
            </RoleBasedRoute>
          </ProtectedRoute>
        } />

        <Route path="/admin-audit-logs" element={
          <ProtectedRoute>
            <RoleBasedRoute
              requiredPermission={Permission.CAN_VIEW_SYSTEM_SETTING}
              fallbackPath="/admin-audit-logs"
              showAccessDenied={true}
            >
              <AdminAuditLogsPage />
            </RoleBasedRoute>
          </ProtectedRoute>
        } />

        <Route path="/admin-settings" element={
          <ProtectedRoute>
            <RoleBasedRoute
              requiredPermission={Permission.CAN_VIEW_SYSTEM_SETTING}
              fallbackPath="/admin-settings"
              showAccessDenied={true}
            >
              <AdminSettingsPage />
            </RoleBasedRoute>
          </ProtectedRoute>
        } />

        <Route path="/admin-roles" element={
          <ProtectedRoute>
            <RoleBasedRoute
              requiredPermission={Permission.CAN_VIEW_SYSTEM_SETTING}
              fallbackPath="/admin-roles"
              showAccessDenied={true}
            >
              <AdminRolePage />
            </RoleBasedRoute>
          </ProtectedRoute>
        } />

        <Route path="/admin-permissions" element={
          <ProtectedRoute>
            <RoleBasedRoute
              requiredPermission={Permission.CAN_VIEW_SYSTEM_SETTING}
              fallbackPath="/admin-permissions"
              showAccessDenied={true}
            >
              <AdminPermissionPage />
            </RoleBasedRoute>
          </ProtectedRoute>
        } />

        <Route path="/admin-employees" element={
          <ProtectedRoute>
            <RoleBasedRoute
              requiredPermission={Permission.CAN_VIEW_SYSTEM_SETTING}
              fallbackPath="/admin-employees"
              showAccessDenied={true}
            >
              <AdminEmployeePage />
            </RoleBasedRoute>
          </ProtectedRoute>
        } />

        <Route path="/admin-devices" element={
          <ProtectedRoute>
            <RoleBasedRoute
              requiredPermission={Permission.CAN_VIEW_DEVICE_SETTING}
              fallbackPath="/admin-devices"
              showAccessDenied={true}
            >
              <AdminManageDevicePage />
            </RoleBasedRoute>
          </ProtectedRoute>
        } />

        <Route path="/admin-form-templates" element={
          <ProtectedRoute>
            <RoleBasedRoute
              requiredPermission={Permission.CAN_VIEW_SYSTEM_SETTING}
              fallbackPath="/admin-form-templates"
              showAccessDenied={true}
            >
              <AdminFormTemplatePage />
            </RoleBasedRoute>
          </ProtectedRoute>
        } />

        <Route path="/admin-workflows" element={
          <ProtectedRoute>
            <RoleBasedRoute
              requiredPermission={Permission.CAN_VIEW_SYSTEM_SETTING}
              fallbackPath="/admin-workflows"
              showAccessDenied={true}
            >
              <AdminWorkflowPage />
            </RoleBasedRoute>
          </ProtectedRoute>
        } />

        <Route path="/admin-document-templates" element={
          <ProtectedRoute>
            <RoleBasedRoute
              requiredPermission={Permission.CAN_VIEW_SYSTEM_SETTING}
              fallbackPath="/admin-document-templates"
              showAccessDenied={true}
            >
              <AdminDocumentTemplatePage />
            </RoleBasedRoute>
          </ProtectedRoute>
        } />

        <Route path="/admin-application-images" element={
          <ProtectedRoute>
            <RoleBasedRoute
              requiredPermission={Permission.CAN_VIEW_SYSTEM_SETTING}
              fallbackPath="/admin-application-images"
              showAccessDenied={true}
            >
              <AdminApplicationImagePage />
            </RoleBasedRoute>
          </ProtectedRoute>
        } />

        <Route path="/admin-outlets" element={
          <ProtectedRoute>
            <RoleBasedRoute
              requiredPermission={Permission.CAN_VIEW_SYSTEM_SETTING}
              fallbackPath="/admin-outlets"
              showAccessDenied={true}
            >
              <AdminOutletPage />
            </RoleBasedRoute>
          </ProtectedRoute>
        } />

        <Route path="/admin-properties" element={
          <ProtectedRoute>
            <RoleBasedRoute
              requiredPermission={Permission.CAN_VIEW_SYSTEM_SETTING}
              fallbackPath="/admin-properties"
              showAccessDenied={true}
            >
              <AdminPropertyPage />
            </RoleBasedRoute>
          </ProtectedRoute>
        } />

        <Route path="/admin-documents-signed" element={
          <ProtectedRoute>
            <RoleBasedRoute
              requiredPermission={Permission.CAN_VIEW_SIGNED_DOCUMENTS}
              fallbackPath="/admin-documents-signed"
              showAccessDenied={true}
            >
              <AdminCustomerSignedPage />
            </RoleBasedRoute>
          </ProtectedRoute>
        } />

        <Route path="/admin-master-data" element={
          <ProtectedRoute>
            <AdminLanguagePatronTypePage />
          </ProtectedRoute>
        } />

      </Routes>
    </>
  );
}

function App() {
  return (
    <ErrorBoundary>
      <Router>
        <AppDataProvider>
          <PageTitleProvider>
            <AppLoadingProvider>
              <SessionManager>
                <React.Suspense
                  fallback={
                    <div
                      style={{
                        minHeight: '100vh',
                        display: 'flex',
                        alignItems: 'center',
                        justifyContent: 'center'
                      }}
                    >
                      Loading...
                    </div>
                  }
                >
                  <AppContent />
                </React.Suspense>
              </SessionManager>
            </AppLoadingProvider>
          </PageTitleProvider>
        </AppDataProvider>
      </Router>
    </ErrorBoundary>
  )
}

export default App