import { api, unwrapApiEnvelope } from './commonApiService';
import { BASE_LANGUAGE, BASE_PATRON_TYPE } from '../constants/baseApiEnpoint';
import type {
    LanguageResponse,
    CreateLanguageRequest,
    UpdateLanguageRequest,
    PatronTypeResponse,
    CreatePatronTypeRequest,
    UpdatePatronTypeRequest,
} from '../types/masterDataType';

export const languageService = {
    getAllAsync: async (): Promise<LanguageResponse[]> =>
        unwrapApiEnvelope(await api.get(`${BASE_LANGUAGE}/all`)),

    getByIdAsync: async (id: number): Promise<LanguageResponse> =>
        unwrapApiEnvelope(await api.get(`${BASE_LANGUAGE}/${id}`)),

    createAsync: async (request: CreateLanguageRequest): Promise<LanguageResponse> =>
        unwrapApiEnvelope(await api.post(`${BASE_LANGUAGE}/create`, request)),

    updateAsync: async (request: UpdateLanguageRequest): Promise<LanguageResponse> =>
        unwrapApiEnvelope(await api.post(`${BASE_LANGUAGE}/update`, request)),

    deleteAsync: async (id: number): Promise<void> =>
        unwrapApiEnvelope(await api.post(`${BASE_LANGUAGE}/delete/${id}`)),

    toggleActiveAsync: async (id: number): Promise<void> =>
        unwrapApiEnvelope(await api.post(`${BASE_LANGUAGE}/${id}/toggle-active`)),
};

export const patronTypeService = {
    getAllAsync: async (): Promise<PatronTypeResponse[]> =>
        unwrapApiEnvelope(await api.get(`${BASE_PATRON_TYPE}/all`)),

    getByIdAsync: async (id: number): Promise<PatronTypeResponse> =>
        unwrapApiEnvelope(await api.get(`${BASE_PATRON_TYPE}/${id}`)),

    createAsync: async (request: CreatePatronTypeRequest): Promise<PatronTypeResponse> =>
        unwrapApiEnvelope(await api.post(`${BASE_PATRON_TYPE}/create`, request)),

    updateAsync: async (request: UpdatePatronTypeRequest): Promise<PatronTypeResponse> =>
        unwrapApiEnvelope(await api.post(`${BASE_PATRON_TYPE}/update`, request)),

    deleteAsync: async (id: number): Promise<void> =>
        unwrapApiEnvelope(await api.post(`${BASE_PATRON_TYPE}/delete/${id}`)),

    toggleActiveAsync: async (id: number): Promise<void> =>
        unwrapApiEnvelope(await api.post(`${BASE_PATRON_TYPE}/${id}/toggle-active`)),
};
