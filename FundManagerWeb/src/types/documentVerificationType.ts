export interface DocumentsPagingRequest {
    Page: number;
    PageSize: number;
    Take?: number;
    Skip?: number;
    SearchTerm?: string;
}

export interface DocumentsPagingResponse {
    totalRecords: number;
    data: PatronDocumentGroup[];
}

export interface PatronDocumentGroup {
    pid: number;
    playerId: number;
    fullName: string;
    firstName: string;
    lastName: string;
    jobTitle: string;
    position: string;
    gender: string;
    birthday: string;
    idNumber: string;
    address: string;
    documentCount: number;
    signedDate: string;
    registrationType: number;
    documents: DocumentResponse[];
}

export interface DocumentResponse {
    id: number;
    fileName: string;
    fileUrl: string;
    uploadedDate: string;
    documentType: string;
    isOnline: boolean;
}

export interface SyncIncomeDocumentRequest {
    OldPlayerId?: number;
    NewPlayerId?: number;
}

export interface SyncIncomeDocumentResponse {
    mappedBatches: number;
    mappedFiles: number;
    errors: string[];
    newFilesUrl: string[];
}