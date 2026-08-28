import type { UploadedFileResponse, UploadImageBase64Request } from "../types/uploadImageType";
import { api, unwrapApiEnvelope } from "./commonApiService";

export const uploadService = {
    uploadImageAsync: async (request: UploadImageBase64Request): Promise<UploadedFileResponse> => {
        const response = await api.post(`/api/Upload/image`, request);
        return unwrapApiEnvelope(response);
    },
}