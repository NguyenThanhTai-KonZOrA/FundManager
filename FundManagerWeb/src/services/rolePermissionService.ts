import type { AssignRoleRequest, CreatePermissionRequest, CreateRoleRequest, EmployeeResponse, EmployeeWithRoles, PermissionResponse, RoleResponse, UpdatePermissionRequest, UpdateRoleRequest } from "../types/rolePermissionType";
import { BASE_ROLE, BASE_PERMISSION, BASE_EMPLOYEE_ROLE } from "../constants/baseApiEnpoint";
import { api, unwrapApiEnvelope } from "./commonApiService";

export const roleService = {
    getAllRolesAsync: async (): Promise<RoleResponse[]> => {
        const response = await api.get(`${BASE_ROLE}/all`);
        return unwrapApiEnvelope(response);
    },

    getRoleByIdAsync: async (id: number): Promise<RoleResponse> => {
        const response = await api.get(`${BASE_ROLE}/${id}`);
        return unwrapApiEnvelope(response);
    },

    createRoleAsync: async (roleData: CreateRoleRequest): Promise<RoleResponse> => {
        const response = await api.post(`${BASE_ROLE}/create`, roleData);
        return unwrapApiEnvelope(response);
    },

    updateRoleAsync: async (id: number, roleData: UpdateRoleRequest): Promise<RoleResponse> => {
        const response = await api.post(`${BASE_ROLE}/update/${id}`, roleData);
        return unwrapApiEnvelope(response);
    },

    deleteRoleAsync: async (id: number): Promise<void> => {
        await api.post(`${BASE_ROLE}/delete/${id}`);
    },

    changeStatusAsync: async (id: number): Promise<RoleResponse> => {
        const response = await api.post(`${BASE_ROLE}/change-status/${id}`);
        return unwrapApiEnvelope(response);
    }
};

export const permissionService = {
    getAllPermissionsAsync: async (): Promise<PermissionResponse[]> => {
        const response = await api.get(`${BASE_PERMISSION}/all`);
        return unwrapApiEnvelope(response);
    },

    getPermissionsByCategoryAsync: async (): Promise<Record<string, PermissionResponse[]>> => {
        const response = await api.get(`${BASE_PERMISSION}/by-category`);
        return unwrapApiEnvelope(response);
    },

    getPermissionByIdAsync: async (id: number): Promise<PermissionResponse> => {
        const response = await api.get(`${BASE_PERMISSION}/${id}`);
        return unwrapApiEnvelope(response);
    },

    createPermissionAsync: async (data: CreatePermissionRequest): Promise<PermissionResponse> => {
        const response = await api.post(`${BASE_PERMISSION}/create`, data);
        return unwrapApiEnvelope(response);
    },

    updatePermissionAsync: async (id: number, data: UpdatePermissionRequest): Promise<PermissionResponse> => {
        const response = await api.post(`${BASE_PERMISSION}/update/${id}`, data);
        return unwrapApiEnvelope(response);
    },

    deletePermissionAsync: async (id: number): Promise<boolean> => {
        const response = await api.post(`${BASE_PERMISSION}/delete/${id}`);
        return unwrapApiEnvelope(response);
    },

    togglePermissionStatusAsync: async (id: number): Promise<boolean> => {
        const response = await api.post(`${BASE_PERMISSION}/change-status/${id}`);
        return unwrapApiEnvelope(response);
    }
};

export const employeeRoleService = {
    getEmployeeWithRolesAsync: async (employeeId: number): Promise<EmployeeWithRoles> => {
        const response = await api.get(`${BASE_EMPLOYEE_ROLE}/roles/${employeeId}`);
        return unwrapApiEnvelope(response);
    },

    assignRolesToEmployeeAsync: async (data: AssignRoleRequest): Promise<boolean> => {
        const response = await api.post(`${BASE_EMPLOYEE_ROLE}/assign-roles`, data);
        return unwrapApiEnvelope(response);
    },

    getEmployeePermissionsAsync: async (employeeId: number): Promise<string[]> => {
        const response = await api.get(`${BASE_EMPLOYEE_ROLE}/permissions/${employeeId}`);
        return unwrapApiEnvelope(response);
    },

    getAllEmployeesAsync: async (): Promise<EmployeeResponse[]> => {
        const response = await api.get(`${BASE_EMPLOYEE_ROLE}/list`);
        return unwrapApiEnvelope(response);
    },
};