export interface UploadImageBase64Request {
    Base64Data: string;
    FileName?: string;
    Context?: string; // "task", "project"
    ContextId?: number; // ID of task or project
}

export interface UploadedFileResponse {
    id: number;
    fileName: string;
    originalFileName: string;
    url: string;
    relativePath: string;
    fileSizeBytes: number;
    fileSizeFormatted: string;
    contentType: string;
    uploadedAt: string;
    context: string; // "task", "project"
    contextId: number; // ID of task or project
}