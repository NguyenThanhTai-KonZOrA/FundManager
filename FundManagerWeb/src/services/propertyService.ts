import { api, unwrapApiEnvelope } from "./commonApiService";
import type {
    PropertyResponse,
    CreatePropertyRequest,
    UpdatePropertyRequest,
    DeviceResponse,
    RegisterDeviceRequest,
    PropertyDeviceMappingResponse,
    CreateMappingRequest,
    UpdateMappingRequest,
    GetPropertyDeviceMappedRequest,
} from "../types/propertyType";

import { BASE_PROPERTY, BASE_MAPPING_PROPERTY } from "../constants/baseApiEnpoint";
// ── Property Service ──────────────────────────────────────────────────────────

export const propertyService = {
    getListAsync: async (): Promise<PropertyResponse[]> => {
        return unwrapApiEnvelope(await api.get(`${BASE_PROPERTY}/list`));
    },

    getDetailAsync: async (id: number): Promise<PropertyResponse> => {
        return unwrapApiEnvelope(await api.get(`${BASE_PROPERTY}/${id}`));
    },

    createAsync: async (request: CreatePropertyRequest): Promise<PropertyResponse> => {
        return unwrapApiEnvelope(await api.post(`${BASE_PROPERTY}/create`, request));
    },

    updateAsync: async (request: UpdatePropertyRequest): Promise<PropertyResponse> => {
        return unwrapApiEnvelope(await api.post(`${BASE_PROPERTY}/update`, request));
    },

    deleteAsync: async (id: number): Promise<void> => {
        return unwrapApiEnvelope(await api.post(`${BASE_PROPERTY}/delete/${id}`));
    },
};

// ── Property Mapping Service ──────────────────────────────────────────────────

export const propertyMappingService = {
    /**
     * Call when FE startup (no auth required).
     * Pass deviceName (hostname) for the backend to query the PropertyDeviceMapping table.
     */
    getPropertyDeviceMappedAsync: async (
        request: GetPropertyDeviceMappedRequest,
    ): Promise<PropertyDeviceMappingResponse | null> => {
        const params: Record<string, string> = {};
        if (request.DeviceName) params.DeviceName = request.DeviceName;
        if (request.MacAddress) params.MacAddress = request.MacAddress;
        const response = await api.get(`${BASE_MAPPING_PROPERTY}/device-mapped`, { params });
        if (response.data?.success === false) return null;
        return response.data?.data ?? null;
    },

    /**
     * Register a new device in the system.
     * If it already exists (same MacAddress), update the information.
     */
    registerDeviceAsync: async (request: RegisterDeviceRequest): Promise<DeviceResponse> => {
        return unwrapApiEnvelope(await api.post(`${BASE_MAPPING_PROPERTY}/register-device`, request));
    },

    getMappingListAsync: async (): Promise<PropertyDeviceMappingResponse[]> => {
        return unwrapApiEnvelope(await api.get(`${BASE_MAPPING_PROPERTY}/list`));
    },

    getMappingDetailAsync: async (id: number): Promise<PropertyDeviceMappingResponse> => {
        return unwrapApiEnvelope(await api.get(`${BASE_MAPPING_PROPERTY}/${id}`));
    },

    /** Create a new mapping - returns an error if the device already has a mapping */
    createMappingAsync: async (request: CreateMappingRequest): Promise<PropertyDeviceMappingResponse> => {
        return unwrapApiEnvelope(await api.post(`${BASE_MAPPING_PROPERTY}/create-mapping`, request));
    },

    updateMappingAsync: async (request: UpdateMappingRequest): Promise<PropertyDeviceMappingResponse> => {
        return unwrapApiEnvelope(await api.post(`${BASE_MAPPING_PROPERTY}/update`, request));
    },

    getAllDevicesAsync: async (): Promise<DeviceResponse[]> => {
        return unwrapApiEnvelope(await api.get(`${BASE_MAPPING_PROPERTY}/devices`));
    },

    getClientNameAsync: async (): Promise<{ ip: string; computerName: string }> => {
        const response = await api.get(`${BASE_MAPPING_PROPERTY}/client-name`);
        return response.data;
    },
};
