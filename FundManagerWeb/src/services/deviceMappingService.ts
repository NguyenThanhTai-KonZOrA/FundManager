import type { CreateMappingRequest, CreateMappingResponse, CurrentHostNameResponse, CurrentStaffDeviceResponse, GetMappingByStaffDeviceResponse, MappingDataResponse, OnlineStaffDevicesResponse, StaffAndPatronDevicesResponse, StaffSignatureRequest, UpdateMappingRequest, UpdateMappingResponse } from "../types/deviceType";
import { BASE_MANAGEMENT_DEVICE, BASE_MAPPING_DEVICE, BASE_PATRON_DEVICE } from "../constants/baseApiEnpoint";
import type { ApiEnvelope } from "../types/commonType";
import type { ChangeHostnameRequest, DeleteDeviceRequest, ManageDeviceResponse, ToggleDeviceRequest } from "../types/deviceType";
import { api, unwrapApiEnvelope } from "./commonApiService";

export const manageDeviceService = {
    getAllDevicesAsync: async (): Promise<ManageDeviceResponse> => {
        const response = await api.get<ApiEnvelope<ManageDeviceResponse>>(`${BASE_MANAGEMENT_DEVICE}/all-devices`);
        return unwrapApiEnvelope(response);
    },

    changeStatusDeviceAsync: async (request: ToggleDeviceRequest): Promise<boolean> => {
        const response = await api.post<ApiEnvelope<boolean>>(`${BASE_MANAGEMENT_DEVICE}/toggle-active`, request);
        return unwrapApiEnvelope(response);
    },

    deleteDeviceAsync: async (request: DeleteDeviceRequest): Promise<boolean> => {
        const response = await api.post<ApiEnvelope<boolean>>(`${BASE_MANAGEMENT_DEVICE}/delete`, request);
        return unwrapApiEnvelope(response);
    },

    changeHostnameAsync: async (request: ChangeHostnameRequest): Promise<boolean> => {
        const response = await api.post<ApiEnvelope<boolean>>(`${BASE_MANAGEMENT_DEVICE}/change-hostname`, request);
        return unwrapApiEnvelope(response);
    }
};

export const mappingDeviceService = {
    createMappingAsync: async (request: CreateMappingRequest): Promise<CreateMappingResponse> => {
        const response = await api.post<ApiEnvelope<CreateMappingResponse>>(`${BASE_MAPPING_DEVICE}/create`, request);
        return unwrapApiEnvelope(response);
    },

    getAllMappingsAsync: async (): Promise<MappingDataResponse[]> => {
        const response = await api.get<ApiEnvelope<MappingDataResponse[]>>(`${BASE_MAPPING_DEVICE}/list`);
        return unwrapApiEnvelope(response);
    },

    getMappingByStaffDeviceAsync: async (staffDeviceName: string): Promise<GetMappingByStaffDeviceResponse | null> => {
        const response = await api.get<ApiEnvelope<GetMappingByStaffDeviceResponse | null>>(`${BASE_MAPPING_DEVICE}/by-staff/${staffDeviceName}`);
        return unwrapApiEnvelope(response);
    },

    updateMappingAsync: async (request: UpdateMappingRequest): Promise<UpdateMappingResponse> => {
        const response = await api.post<ApiEnvelope<UpdateMappingResponse>>(`${BASE_MAPPING_DEVICE}/update`, request);
        return unwrapApiEnvelope(response);
    },

    getStaffAndPatronDevicesAsync: async (): Promise<StaffAndPatronDevicesResponse> => {
        const response = await api.get<ApiEnvelope<StaffAndPatronDevicesResponse>>(`${BASE_MAPPING_DEVICE}/staff-and-patron-devices`);
        return unwrapApiEnvelope(response);
    },

    deleteMappingAsync: async (mappingId: number): Promise<void> => {
        const response = await api.delete<ApiEnvelope<void>>(`${BASE_MAPPING_DEVICE}/delete/${mappingId}`);
        return unwrapApiEnvelope(response);
    }
};

export const staffDeviceService = {
    getCurrentStaffDeviceAsync: async (): Promise<CurrentStaffDeviceResponse> => {
        const response = await api.get<ApiEnvelope<CurrentStaffDeviceResponse>>(`${BASE_PATRON_DEVICE}/current-staff-device`);
        return unwrapApiEnvelope(response);
    },

    getOnlineStaffDevicesAsync: async (): Promise<OnlineStaffDevicesResponse[]> => {
        const response = await api.get<ApiEnvelope<OnlineStaffDevicesResponse[]>>(`${BASE_PATRON_DEVICE}/online-staff-devices`);
        return unwrapApiEnvelope(response);
    },
    getCurrentHostNameAsync: async (): Promise<CurrentHostNameResponse> => {
        const res = await api.get(`${BASE_PATRON_DEVICE}/client-name`);
        return unwrapApiEnvelope(res);
    }
};