import { getApiBase } from "./envConfig";

/**
 * Get the display name for an attachment, ensuring proper file extension.
 */
export const getAttachmentDisplayName = (attachment: { fileName: string; fileExtension: string }): string => {
    const extension = attachment.fileExtension.startsWith(".")
        ? attachment.fileExtension
        : `.${attachment.fileExtension}`;

    if (attachment.fileName.toLowerCase().endsWith(extension.toLowerCase())) {
        return attachment.fileName;
    }

    return `${attachment.fileName}${extension}`;
};

/**
 * Get the MIME type for a file extension.
 */
export const getAttachmentMimeType = (extension: string): string => {
    const normalized = extension.replace(".", "").toLowerCase();
    const mimeTypeMap: Record<string, string> = {
        pdf: "application/pdf",
        jpg: "image/jpeg",
        jpeg: "image/jpeg",
        png: "image/png",
        gif: "image/gif",
        webp: "image/webp",
        svg: "image/svg+xml",
        mp4: "video/mp4",
        webm: "video/webm",
        ogg: "video/ogg",
        mov: "video/quicktime",
        avi: "video/x-msvideo",
    };

    return mimeTypeMap[normalized] || "application/octet-stream";
};

/**
 * Normalize an attachment URL to include the API base if it's a relative path.
 */
export const normalizeAttachmentUrl = (url: string): string => {
    if (url.startsWith("http://") || url.startsWith("https://")) {
        return url;
    }
    return `${getApiBase()}${url}`;
};
