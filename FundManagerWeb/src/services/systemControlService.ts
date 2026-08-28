import { api, unwrapApiEnvelope } from "./commonApiService";
import type { CreateSettingsRequest, SettingsInfoResponse, SettingsResponse, UpdateSettingsRequest, UpdateSettingsResponse, ClearCacheSettingResponse } from "../types/settingsType";
import { BASE_SETTINGS } from "../constants/baseApiEnpoint";

export const settingsService = {
    getAllSettingsAsync: async (): Promise<SettingsResponse[]> => {
        const res = await api.get(`${BASE_SETTINGS}/all`);
        return unwrapApiEnvelope(res);
    },

    createSettingsAsync: async (data: CreateSettingsRequest): Promise<boolean> => {
        const res = await api.post(`${BASE_SETTINGS}/create`, data);
        return unwrapApiEnvelope(res);
    },

    getSettingsInfoAsync: async (): Promise<SettingsInfoResponse> => {
        const res = await api.get(`${BASE_SETTINGS}/info`);
        return unwrapApiEnvelope(res);
    },

    clearCacheSettingAsync: async (key: string): Promise<ClearCacheSettingResponse> => {
        const res = await api.post(`${BASE_SETTINGS}/clear-cache/${key}`);
        return unwrapApiEnvelope(res);
    },

    getSettingDetailAsync: async (key: string): Promise<SettingsResponse> => {
        const res = await api.get(`${BASE_SETTINGS}/${key}`);
        return unwrapApiEnvelope(res);
    },

    updateSettingAsync: async (key: string, data: UpdateSettingsRequest): Promise<UpdateSettingsResponse> => {
        const res = await api.post(`${BASE_SETTINGS}/${key}`, data);
        return unwrapApiEnvelope(res);
    }
};