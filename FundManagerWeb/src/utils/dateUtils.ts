/**
 * Returns today's date in Vietnam timezone (UTC+7) as YYYY-MM-DD string.
 * Use this instead of `new Date().toISOString().split('T')[0]` for date inputs
 * so that the correct local date is shown after midnight Vietnam time.
 */
export const getTodayVN = (): string => {
    return new Intl.DateTimeFormat('sv-SE', { timeZone: 'Asia/Ho_Chi_Minh' }).format(new Date());
};

/**
 * Format a date string into a readable format (e.g., "01 Jan 2026").
 */
export const formatDate = (dateString: string | Date): string => {
    const date = new Date(dateString);
    return new Intl.DateTimeFormat("en-US", {
        day: "2-digit",
        month: "short",
        year: "numeric",
    }).format(date);
};

/**
 * Format a date string into a readable format (e.g., "01-02-2026").
 */
export const formatDateDDMMYYYY = (dateStr?: string | Date): string => {
    if (!dateStr) return "";

    let d: Date;
    if (typeof dateStr === 'string') {
        if (dateStr === "0001-01-01T00:00:00") return "-";
        // Clean up the date string - remove milliseconds if present
        let cleanDateStr = dateStr.split('.')[0];
        // Replace space with 'T' if needed for ISO format
        if (cleanDateStr.includes(' ') && !cleanDateStr.includes('T')) {
            cleanDateStr = cleanDateStr.replace(' ', 'T');
        }
        d = new Date(cleanDateStr);
    } else {
        d = dateStr;
    }
    if (isNaN(d.getTime())) return "";

    const day = String(d.getDate()).padStart(2, "0");
    const month = String(d.getMonth() + 1).padStart(2, "0");
    const year = d.getFullYear();
    return `${day}-${month}-${year}`;
}

export const formatDateYYYYMMDD = (dateStr?: string | Date): string => {
    if (!dateStr) return "";

    let d: Date;
    if (typeof dateStr === 'string') {
        if (dateStr === "0001-01-01T00:00:00") return "-";
        // Clean up the date string - remove milliseconds if present
        let cleanDateStr = dateStr.split('.')[0];
        // Replace space with 'T' if needed for ISO format
        if (cleanDateStr.includes(' ') && !cleanDateStr.includes('T')) {
            cleanDateStr = cleanDateStr.replace(' ', 'T');
        }
        d = new Date(cleanDateStr);
    } else {
        d = dateStr;
    }
    if (isNaN(d.getTime())) return "";

    const day = String(d.getDate()).padStart(2, "0");
    const month = String(d.getMonth() + 1).padStart(2, "0");
    const year = d.getFullYear();
    return `${year}-${month}-${day}`;
}

export const formatDateDDMMMYYYY = (dateStr?: string | Date): string => {
    if (!dateStr) return "";

    let d: Date;

    if (typeof dateStr === 'string') {
        if (dateStr === "0001-01-01T00:00:00") return "-";

        // Clean up the date string - remove milliseconds if present
        let cleanDateStr = dateStr.split('.')[0];

        // Replace space with 'T' if needed for ISO format
        // if (cleanDateStr.includes(' ') && !cleanDateStr.includes('T')) {
        //   cleanDateStr = cleanDateStr.replace(' ', 'T');
        // }

        // Server returns UTC time without 'Z' suffix, so we need to explicitly treat it as UTC
        let utcDateStr = cleanDateStr;

        // If the date string doesn't end with 'Z' or have timezone info, append 'Z' to indicate UTC
        // if (!cleanDateStr.endsWith('Z') && !cleanDateStr.includes('+') && !cleanDateStr.includes('-', 10)) {
        //   utcDateStr = cleanDateStr + 'Z';
        // }

        d = new Date(utcDateStr);
    } else {
        d = dateStr;
    }

    if (isNaN(d.getTime())) return "";

    // Month names in short format
    const monthNames = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun',
        'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];

    const day = String(d.getDate()).padStart(2, "0");
    const month = monthNames[d.getMonth()];
    const year = d.getFullYear();

    return `${day}-${month}-${year}`;
}

/**
 * Get a human-readable "time ago" string from a date.
 */
export const getTimeAgo = (dateString: string | Date): string => {
    const date = new Date(dateString);
    const now = new Date();
    const diffMs = now.getTime() - date.getTime();
    const diffMins = Math.floor(diffMs / 60000);
    const diffHours = Math.floor(diffMins / 60);
    const diffDays = Math.floor(diffHours / 24);

    if (diffMins < 1) return "just now";
    if (diffMins < 60) return `${diffMins} minute${diffMins > 1 ? "s" : ""} ago`;
    if (diffHours < 24) return `${diffHours} hour${diffHours > 1 ? "s" : ""} ago`;
    return `${diffDays} day${diffDays > 1 ? "s" : ""} ago`;
};

/**
 * Convert a local datetime-local input (YYYY-MM-DD or YYYY-MM-DDTHH:mm[:ss])
 * into a UTC ISO-like string without the trailing Z (e.g. 2026-07-29T17:00:00)
 * which matches the server's UTC-without-Z format used elsewhere in the app.
 */
export const localToUtcStringNoZ = (local: string | undefined | null, endOfDay = false): string | undefined => {
    if (!local) return undefined;

    // Accepts YYYY-MM-DD or YYYY-MM-DDTHH:mm or YYYY-MM-DDTHH:mm:ss
    const m = /^([0-9]{4})-([0-9]{2})-([0-9]{2})(?:T([0-9]{2}):([0-9]{2})(?::([0-9]{2}))?)?$/.exec(local);
    if (!m) return local as string;

    const year = Number(m[1]);
    const month = Number(m[2]);
    const day = Number(m[3]);
    let hour = m[4] ? Number(m[4]) : 0;
    let minute = m[5] ? Number(m[5]) : 0;
    let second = m[6] ? Number(m[6]) : 0;

    if (endOfDay && !m[4]) {
        hour = 23;
        minute = 59;
        second = 59;
    }

    // Construct a Date in the local timezone
    const localDate = new Date(year, month - 1, day, hour, minute, second);
    const iso = localDate.toISOString(); // e.g. 2026-07-29T17:00:00.000Z
    return iso.split('.')[0]; // remove millis and trailing Z -> 2026-07-29T17:00:00
};
