import { api, unwrapApiEnvelope } from "./commonApiService";
import type {
    ApplicationImageResponse,
    CreateApplicationImageFormData,
    UpdateApplicationImageFormData,
} from "../types/applicationImageType";
import { ImageTypeEnum } from "../types/applicationImageType";

const BASE_APPLICATION = "/api/application-image";

function toFormData(data: Record<string, unknown>): FormData {
    const fd = new FormData();
    for (const [key, value] of Object.entries(data)) {
        if (value === null || value === undefined) continue;
        if (value instanceof File) {
            fd.append(key, value);
        } else {
            fd.append(key, String(value));
        }
    }
    return fd;
}

export const applicationImageService = {
    getListAsync: async (): Promise<ApplicationImageResponse[]> => {
        return unwrapApiEnvelope(await api.get(`${BASE_APPLICATION}/list`));
    },

    getByTypeAsync: async (type: ImageTypeEnum): Promise<ApplicationImageResponse[]> => {
        return unwrapApiEnvelope(await api.get(`${BASE_APPLICATION}/by-type/${type}`));
    },

    getDetailAsync: async (id: number): Promise<ApplicationImageResponse> => {
        return unwrapApiEnvelope(await api.get(`${BASE_APPLICATION}/${id}`));
    },

    createAsync: async (request: CreateApplicationImageFormData): Promise<ApplicationImageResponse> => {
        const fd = toFormData(request as unknown as Record<string, unknown>);
        return unwrapApiEnvelope(await api.post(`${BASE_APPLICATION}/create`, fd, {
            headers: { "Content-Type": "multipart/form-data" },
        }));
    },

    updateAsync: async (request: UpdateApplicationImageFormData): Promise<ApplicationImageResponse> => {
        const fd = toFormData(request as unknown as Record<string, unknown>);
        return unwrapApiEnvelope(await api.post(`${BASE_APPLICATION}/update`, fd, {
            headers: { "Content-Type": "multipart/form-data" },
        }));
    },

    deleteAsync: async (id: number): Promise<void> => {
        return unwrapApiEnvelope(await api.post(`${BASE_APPLICATION}/delete/${id}`));
    },
};
