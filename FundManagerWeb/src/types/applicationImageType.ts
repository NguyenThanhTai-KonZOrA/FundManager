// ── ApplicationImage ──────────────────────────────────────────────────────────

export const ImageTypeEnum = {
    Outlet: 0,
    Slider: 1,       // legacy – kept for backward compat
    Logo: 2,
    Background: 3,
    Icon: 4,
    Other: 5,
    SliderHotel: 6,  // Hotel / property-wide slideshow
    SliderOutlet: 7, // Outlet-scoped slideshow
} as const;

export type ImageTypeEnum = typeof ImageTypeEnum[keyof typeof ImageTypeEnum];

export const IMAGE_TYPE_LABELS: Record<ImageTypeEnum, string> = {
    [ImageTypeEnum.Outlet]: "Outlet Image",
    [ImageTypeEnum.Slider]: "Slider Image (Legacy)",
    [ImageTypeEnum.Logo]: "Logo Image",
    [ImageTypeEnum.Background]: "Background Image",
    [ImageTypeEnum.Icon]: "Icon Image",
    [ImageTypeEnum.Other]: "Other Image",
    [ImageTypeEnum.SliderHotel]: "Slider – Hotel",
    [ImageTypeEnum.SliderOutlet]: "Slider – Outlet",
};

/** Types that require PropertyId */
export const SLIDER_TYPES_REQUIRE_PROPERTY: ImageTypeEnum[] = [
    ImageTypeEnum.Slider,
    ImageTypeEnum.SliderHotel,
];

/** Types that require OutletId */
export const SLIDER_TYPES_REQUIRE_OUTLET: ImageTypeEnum[] = [
    ImageTypeEnum.Outlet,
    ImageTypeEnum.SliderOutlet,
];

export interface ApplicationImageResponse {
    id: number;
    name: string;
    description: string;
    filePath: string;
    fileUrl: string;
    fileExtension: string;
    fileSize: number;
    type: ImageTypeEnum;
    typeName: string;
    propertyId: number | null;
    outletId: number | null;
    isActive: boolean;
    createdAt: string;
    updatedAt: string;
}

/** Sent as multipart/form-data */
export interface CreateApplicationImageFormData {
    Name: string;
    Description: string;
    File: File;
    Type: ImageTypeEnum;
    PropertyId?: number | null;
    OutletId?: number | null;
}

/** Sent as multipart/form-data; File is optional */
export interface UpdateApplicationImageFormData {
    Id: number;
    Name: string;
    Description: string;
    File?: File | null;
    Type: ImageTypeEnum;
    PropertyId?: number | null;
    OutletId?: number | null;
    IsActive: boolean;
}
