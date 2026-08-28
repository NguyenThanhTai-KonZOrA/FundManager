import { api, unwrapApiEnvelope } from "./commonApiService";
import type {
    OutletResponse,
    CreateOutletRequest,
    UpdateOutletRequest,
} from "../types/outletType";
import { BASE_OUTLET } from "../constants/baseApiEnpoint";

export const outletService = {
    getListAsync: async (): Promise<OutletResponse[]> => {
        return unwrapApiEnvelope(await api.get(`${BASE_OUTLET}/list`));
    },

    getByPropertyAsync: async (propertyId: number): Promise<OutletResponse[]> => {
        return unwrapApiEnvelope(await api.get(`${BASE_OUTLET}/by-property/${propertyId}`));
    },

    getDetailAsync: async (id: number): Promise<OutletResponse> => {
        return unwrapApiEnvelope(await api.get(`${BASE_OUTLET}/${id}`));
    },

    createAsync: async (request: CreateOutletRequest): Promise<OutletResponse> => {
        return unwrapApiEnvelope(await api.post(`${BASE_OUTLET}/create`, request));
    },

    updateAsync: async (request: UpdateOutletRequest): Promise<OutletResponse> => {
        return unwrapApiEnvelope(await api.post(`${BASE_OUTLET}/update`, request));
    },

    deleteAsync: async (id: number): Promise<void> => {
        return unwrapApiEnvelope(await api.post(`${BASE_OUTLET}/delete/${id}`));
    },
};