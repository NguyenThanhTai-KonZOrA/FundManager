import {
    Box,
    Button,
    Card,
    IconButton,
    Tab,
    Tabs,
    TextField,
    Tooltip,
    Typography,
    CircularProgress,
    Divider,
    Paper,
    List,
    ListItemButton,
    ListItemAvatar,
    Avatar,
    ListItemText,
    Popper,
    ClickAwayListener,
    Dialog,
    DialogContent,
} from "@mui/material";
import {
    Edit as EditIcon,
    FormatBold as BoldIcon,
    FormatItalic as ItalicIcon,
    FormatListBulleted as BulletIcon,
    FormatListNumbered as NumberedIcon,
    Code as CodeIcon,
    Close as CloseIcon,
    Check as CheckIcon,
    Title as HeadingIcon,
    Link as LinkIcon,
    TableChart as TableIcon,
    CheckBox as TaskListIcon,
    Fullscreen as FullscreenIcon,
    FullscreenExit as FullscreenExitIcon,
    AlternateEmail as MentionIcon,
    Image as ImageIcon,
} from "@mui/icons-material";
import { useState, useRef, useCallback, useMemo } from "react";
import DOMPurify from 'dompurify';
import { logError } from "../utils/errorHandler";
import { uploadService } from "../services/uploadService";
import { normalizeAttachmentUrl } from "../utils/attachmentUtils";

export interface MemberOption {
    memberId: number;
    memberName: string;
    memberImage?: string;
}

interface DescriptionEditorProps {
    /** Initial description text */
    initialDescription: string;
    /** Title to display above the description */
    title?: string;
    /** Placeholder text when description is empty */
    placeholder?: string;
    /** Function to call when saving the description */
    onSave: (newDescription: string) => Promise<void>;
    /** Whether the editor is in read-only mode */
    readOnly?: boolean;
    /** Custom class name */
    className?: string;
    /** Available members for @mention functionality */
    members?: MemberOption[];
    /** Context for image uploads: "task" or "project" */
    context?: 'task' | 'project';
    /** ID of the task or project for image uploads */
    contextId?: number;
}

/**
 * A reusable description editor component similar to GitHub's description editor.
 * Supports Write/Preview tabs, text formatting, mentions, and save/cancel actions.
 * 
 * @example
 * ```tsx
 * <DescriptionEditor
 *   initialDescription={project.description}
 *   title="Description"
 *   members={projectMembers}
 *   onSave={async (newDesc) => {
 *     await projectManagementService.changeProjectDescription({
 *       projectId: project.id,
 *       newDescription: newDesc
 *     });
 *   }}
 * />
 * ```
 */
export const DescriptionEditor: React.FC<DescriptionEditorProps> = ({
    initialDescription,
    title = "Description",
    placeholder = "Add a description...",
    onSave,
    readOnly = false,
    className,
    members = [],
    context = 'task',
    contextId = context === 'task' ? 0 : undefined, // Default to 0 for tasks if not provided
}) => {
    const [isEditing, setIsEditing] = useState(false);
    const [description, setDescription] = useState(initialDescription);
    const [activeTab, setActiveTab] = useState<"write" | "preview">("write");
    const [isSaving, setIsSaving] = useState(false);
    const [isFullscreen, setIsFullscreen] = useState(false);
    const [isUploadingImage, setIsUploadingImage] = useState(false);
    const [previewImg, setPreviewImg] = useState<string | null>(null);
    const textFieldRef = useRef<HTMLTextAreaElement>(null);
    const fileInputRef = useRef<HTMLInputElement>(null);

    // Undo / Redo stack (toolbar actions break browser native undo on controlled inputs)
    const undoStackRef = useRef<string[]>([]);
    const redoStackRef = useRef<string[]>([]);
    const burstStartRef = useRef<string | null>(null); // value at start of current typing burst
    const burstTimerRef = useRef<ReturnType<typeof setTimeout> | null>(null);

    // Mention autocomplete state
    const [mentionSearch, setMentionSearch] = useState("");
    const [mentionAnchor, setMentionAnchor] = useState<{
        element: HTMLElement;
        top: number;
        left: number;
    } | null>(null);
    const [mentionStartPos, setMentionStartPos] = useState<number>(-1);
    const [selectedMentionIndex, setSelectedMentionIndex] = useState(0);

    const filteredMembers = useMemo(() =>
        mentionSearch
            ? members.filter((m) =>
                m.memberName.toLowerCase().includes(mentionSearch.toLowerCase())
            )
            : members,
        [members, mentionSearch]);

    const handleEdit = () => {
        setDescription(initialDescription);
        setIsEditing(true);
        setActiveTab("write");
        undoStackRef.current = [];
        redoStackRef.current = [];
        burstStartRef.current = null;
        if (burstTimerRef.current) clearTimeout(burstTimerRef.current);
    };

    const handleCancel = () => {
        setDescription(initialDescription);
        setIsEditing(false);
        setActiveTab("write");
        setIsFullscreen(false);
        setMentionAnchor(null);
        undoStackRef.current = [];
        redoStackRef.current = [];
        burstStartRef.current = null;
        if (burstTimerRef.current) {
            clearTimeout(burstTimerRef.current);
            burstTimerRef.current = null;
        }
    };

    const handleSave = async () => {
        if (description.trim() === initialDescription.trim()) {
            setIsEditing(false);
            setIsFullscreen(false);
            return;
        }

        setIsSaving(true);
        try {
            await onSave(description.trim());
            setIsEditing(false);
            setIsFullscreen(false);
        } catch (error) {
            logError('DescriptionEditor.handleSave', error);
        } finally {
            setIsSaving(false);
        }
    };

    /**
     * Apply a programmatic description change and record an undo snapshot.
     * All toolbar actions should use this instead of setDescription directly.
     */
    const setDescriptionWithUndo = (newValue: string) => {
        undoStackRef.current.push(description);
        if (undoStackRef.current.length > 100) undoStackRef.current.shift();
        redoStackRef.current = [];
        // Commit any in-progress typing burst
        if (burstTimerRef.current) {
            clearTimeout(burstTimerRef.current);
            burstTimerRef.current = null;
        }
        burstStartRef.current = null;
        setDescription(newValue);
    };

    const insertFormatting = (before: string, after: string = "", placeholder: string = "") => {
        const textarea = textFieldRef.current;
        if (!textarea) return;

        const start = textarea.selectionStart;
        const end = textarea.selectionEnd;
        const selectedText = description.substring(start, end);
        const textToInsert = selectedText || placeholder;
        const newText =
            description.substring(0, start) +
            before +
            textToInsert +
            after +
            description.substring(end);

        setDescriptionWithUndo(newText);

        // Restore cursor position
        setTimeout(() => {
            textarea.focus();
            const cursorPos = start + before.length + textToInsert.length;
            textarea.setSelectionRange(cursorPos, cursorPos);
        }, 0);
    };

    const handleBold = () => insertFormatting("**", "**", "bold text");
    const handleItalic = () => insertFormatting("*", "*", "italic text");
    const handleCode = () => insertFormatting("`", "`", "code");

    const handleHeading = (level: number) => {
        const textarea = textFieldRef.current;
        if (!textarea) return;
        const start = textarea.selectionStart;
        const lineStart = description.lastIndexOf("\n", start - 1) + 1;
        const lineEnd = description.indexOf("\n", start);
        const actualLineEnd = lineEnd === -1 ? description.length : lineEnd;
        const currentLine = description.substring(lineStart, actualLineEnd);

        // Check existing heading level to support toggle (undo)
        const existingHeadingMatch = currentLine.match(/^(#+)\s/);
        const existingLevel = existingHeadingMatch ? existingHeadingMatch[1].length : 0;

        // Remove existing heading markers
        const cleanedLine = currentLine.replace(/^#+\s*/, "");

        // Toggle: if same level clicked again, remove heading; otherwise apply new heading
        const headingMarker = existingLevel === level ? "" : "#".repeat(level) + " ";

        const newText =
            description.substring(0, lineStart) +
            headingMarker +
            cleanedLine +
            description.substring(actualLineEnd);

        setDescriptionWithUndo(newText);
        setTimeout(() => textarea.focus(), 0);
    };

    const handleLink = () => {
        insertFormatting("[", "](url)", "link text");
    };

    const handleTable = () => {
        const tableTemplate = "\n| Header 1 | Header 2 | Header 3 |\n|----------|----------|----------|\n| Cell 1   | Cell 2   | Cell 3   |\n| Cell 4   | Cell 5   | Cell 6   |\n\n";
        const textarea = textFieldRef.current;
        if (!textarea) return;
        const start = textarea.selectionStart;
        const newText =
            description.substring(0, start) +
            tableTemplate +
            description.substring(start);
        setDescriptionWithUndo(newText);
        setTimeout(() => textarea.focus(), 0);
    };

    const handleTaskList = () => {
        const textarea = textFieldRef.current;
        if (!textarea) return;
        const start = textarea.selectionStart;
        const lineStart = description.lastIndexOf("\n", start - 1) + 1;
        const newText =
            description.substring(0, lineStart) +
            "- [ ] " +
            description.substring(lineStart);
        setDescriptionWithUndo(newText);
        setTimeout(() => textarea.focus(), 0);
    };

    const handleBulletList = () => {
        const textarea = textFieldRef.current;
        if (!textarea) return;
        const start = textarea.selectionStart;
        const end = textarea.selectionEnd;
        let newText: string;

        if (start === end) {
            const lineStart = description.lastIndexOf("\n", start - 1) + 1;
            newText =
                description.substring(0, lineStart) +
                "• " +
                description.substring(lineStart);
        } else {
            const selectedText = description.substring(start, end);
            const lines = selectedText.split("\n");
            const bulletedLines = lines.map((line) =>
                line.trim() ? `• ${line.replace(/^[•\-\*]\s*/, "")}` : line
            ).join("\n");
            newText =
                description.substring(0, start) +
                bulletedLines +
                description.substring(end);
        }
        setDescriptionWithUndo(newText);
        setTimeout(() => textarea.focus(), 0);
    };

    const handleNumberedList = () => {
        const textarea = textFieldRef.current;
        if (!textarea) return;
        const start = textarea.selectionStart;
        const end = textarea.selectionEnd;
        let newText: string;

        if (start === end) {
            const lineStart = description.lastIndexOf("\n", start - 1) + 1;
            const prevLineStart = description.lastIndexOf("\n", lineStart - 2) + 1;
            const prevLine = description.substring(prevLineStart, lineStart - 1);
            const numberMatch = prevLine.match(/^(\d+)\.\s/);
            const nextNumber = numberMatch ? parseInt(numberMatch[1]) + 1 : 1;
            newText =
                description.substring(0, lineStart) +
                `${nextNumber}. ` +
                description.substring(lineStart);
        } else {
            const selectedText = description.substring(start, end);
            const lines = selectedText.split("\n");
            let counter = 1;
            const numberedLines = lines.map((line) => {
                if (line.trim()) {
                    const cleanedLine = line.replace(/^\d+\.\s*/, "").replace(/^[•\-\*]\s*/, "");
                    return `${counter++}. ${cleanedLine}`;
                }
                return line;
            }).join("\n");
            newText =
                description.substring(0, start) +
                numberedLines +
                description.substring(end);
        }
        setDescriptionWithUndo(newText);
        setTimeout(() => textarea.focus(), 0);
    };

    // Handle image file select: upload to server and insert markdown image link at cursor
    const handleImageFileSelect = useCallback((e: React.ChangeEvent<HTMLInputElement>) => {
        const files = e.target.files;
        if (!files) return;
        Array.from(files).forEach(file => {
            if (!file.type.startsWith('image/')) return;
            if (file.size > 5 * 1024 * 1024) return; // 5MB limit
            const textarea = textFieldRef.current;
            const cursorPos = textarea?.selectionStart ?? description.length;
            const placeholder = `![uploading...]() `;
            setDescription(prev => prev.substring(0, cursorPos) + placeholder + prev.substring(cursorPos));
            setIsUploadingImage(true);
            const reader = new FileReader();
            reader.onload = async (event) => {
                try {
                    const base64DataUrl = event.target?.result as string;
                    const base64Data = base64DataUrl.split(',')[1];
                    const result = await uploadService.uploadImageAsync({
                        Base64Data: base64Data,
                        FileName: file.name || 'uploaded-image.png',
                        Context: 'other',
                        ContextId: contextId,
                    });
                    const imageMarkdown = `![image](${result.url})`;
                    setDescription(prev => prev.replace(placeholder, imageMarkdown));
                } catch (err) {
                    logError('DescriptionEditor.handleImageFileSelect', err);
                    setDescription(prev => prev.replace(placeholder, ''));
                } finally {
                    setIsUploadingImage(false);
                }
            };
            reader.readAsDataURL(file);
        });
        if (fileInputRef.current) fileInputRef.current.value = '';
    }, [description, contextId]);

    // Handle image paste: upload to server and insert markdown image link at cursor
    const handlePaste = useCallback(async (e: React.ClipboardEvent) => {
        const items = e.clipboardData?.items;
        if (!items) return;
        for (let i = 0; i < items.length; i++) {
            if (items[i].type.startsWith('image/')) {
                e.preventDefault();
                const file = items[i].getAsFile();
                if (!file) break;
                if (file.size > 5 * 1024 * 1024) break; // 5MB limit
                const textarea = textFieldRef.current;
                const cursorPos = textarea?.selectionStart ?? description.length;
                const placeholder = `![uploading...]()`;
                setDescription(prev => prev.substring(0, cursorPos) + placeholder + prev.substring(cursorPos));
                setIsUploadingImage(true);
                const reader = new FileReader();
                reader.onload = async (event) => {
                    try {
                        const base64DataUrl = event.target?.result as string;
                        const base64Data = base64DataUrl.split(',')[1];
                        const result = await uploadService.uploadImageAsync({
                            Base64Data: base64Data,
                            FileName: file.name || 'pasted-image.png',
                            Context: 'other',
                            ContextId: contextId,
                        });
                        const imageMarkdown = `![image](${result.url})`;
                        setDescription(prev => prev.replace(placeholder, imageMarkdown));
                    } catch (err) {
                        logError('DescriptionEditor.handlePaste', err);
                        // Remove placeholder on failure
                        setDescription(prev => prev.replace(placeholder, ''));
                    } finally {
                        setIsUploadingImage(false);
                    }
                };
                reader.readAsDataURL(file);
                break;
            }
        }
    }, [description, context, contextId]);

    // Handle mention functionality
    const handleTextChange = (e: React.ChangeEvent<HTMLTextAreaElement>) => {
        const newValue = e.target.value;

        // Track typing burst: save the pre-burst description on the first keystroke
        // so Ctrl+Z can restore the state before the burst started
        if (burstStartRef.current === null) {
            burstStartRef.current = description; // capture value BEFORE this keystroke
        }
        if (burstTimerRef.current) clearTimeout(burstTimerRef.current);
        burstTimerRef.current = setTimeout(() => {
            // Burst ended — push pre-burst snapshot to undo stack
            if (burstStartRef.current !== null) {
                undoStackRef.current.push(burstStartRef.current);
                if (undoStackRef.current.length > 100) undoStackRef.current.shift();
            }
            burstStartRef.current = null;
            burstTimerRef.current = null;
        }, 1000);
        redoStackRef.current = [];

        setDescription(newValue);

        if (members.length === 0) return;

        // Check for @ mention trigger (supports Unicode/accented characters)
        const cursorPos = e.target.selectionStart;
        const textBeforeCursor = newValue.substring(0, cursorPos);
        const lastAtPos = textBeforeCursor.lastIndexOf('@');

        if (lastAtPos !== -1) {
            const searchText = textBeforeCursor.substring(lastAtPos + 1);
            // Show mention list as long as there's no space after @ (match first token)
            if (!searchText.includes(' ')) {
                setMentionSearch(searchText);
                setMentionStartPos(lastAtPos);
                setSelectedMentionIndex(0);

                const textarea = textFieldRef.current;
                if (textarea) {
                    setMentionAnchor({
                        element: textarea,
                        top: textarea.offsetTop,
                        left: textarea.offsetLeft,
                    });
                }
            } else {
                setMentionAnchor(null);
            }
        } else {
            setMentionAnchor(null);
        }
    };

    const insertMention = (member: MemberOption) => {
        if (mentionStartPos === -1) return;

        const textarea = textFieldRef.current;
        if (!textarea) return;

        const cursorPos = textarea.selectionStart;
        const beforeMention = description.substring(0, mentionStartPos);
        const afterMention = description.substring(cursorPos);
        // Use @"Full Name" format for names with spaces (matches CommentSection format)
        const mentionText = member.memberName.includes(' ')
            ? `@"${member.memberName}"`
            : `@${member.memberName}`;

        const newText = beforeMention + mentionText + " " + afterMention;
        setDescriptionWithUndo(newText);
        setMentionAnchor(null);
        setMentionStartPos(-1);

        setTimeout(() => {
            textarea.focus();
            const newCursorPos = mentionStartPos + mentionText.length + 1;
            textarea.setSelectionRange(newCursorPos, newCursorPos);
        }, 0);
    };

    const handleMentionKeyDown = (e: React.KeyboardEvent) => {
        if (!mentionAnchor || filteredMembers.length === 0) return;

        if (e.key === "ArrowDown") {
            e.preventDefault();
            setSelectedMentionIndex((prev) =>
                prev < filteredMembers.length - 1 ? prev + 1 : prev
            );
        } else if (e.key === "ArrowUp") {
            e.preventDefault();
            setSelectedMentionIndex((prev) => (prev > 0 ? prev - 1 : 0));
        } else if (e.key === "Enter" || e.key === "Tab") {
            e.preventDefault();
            insertMention(filteredMembers[selectedMentionIndex]);
        } else if (e.key === "Escape") {
            e.preventDefault();
            setMentionAnchor(null);
        }
    };

    // Keyboard shortcuts
    const handleKeyDown = (e: React.KeyboardEvent) => {
        if (mentionAnchor) {
            handleMentionKeyDown(e);
            return;
        }

        if (e.ctrlKey || e.metaKey) {
            const key = e.key.toLowerCase();

            // Undo: Ctrl+Z
            if (key === 'z' && !e.shiftKey) {
                e.preventDefault();
                // If mid-burst, commit burst snapshot first as the undo target
                if (burstStartRef.current !== null) {
                    if (burstTimerRef.current) {
                        clearTimeout(burstTimerRef.current);
                        burstTimerRef.current = null;
                    }
                    const prev = burstStartRef.current;
                    burstStartRef.current = null;
                    redoStackRef.current.push(description);
                    setDescription(prev);
                } else if (undoStackRef.current.length > 0) {
                    redoStackRef.current.push(description);
                    if (redoStackRef.current.length > 100) redoStackRef.current.shift();
                    setDescription(undoStackRef.current.pop()!);
                }
                return;
            }

            // Redo: Ctrl+Shift+Z or Ctrl+Y
            if ((key === 'z' && e.shiftKey) || key === 'y') {
                e.preventDefault();
                if (redoStackRef.current.length > 0) {
                    undoStackRef.current.push(description);
                    if (undoStackRef.current.length > 100) undoStackRef.current.shift();
                    setDescription(redoStackRef.current.pop()!);
                }
                return;
            }

            switch (key) {
                case "b":
                    e.preventDefault();
                    handleBold();
                    break;
                case "i":
                    e.preventDefault();
                    handleItalic();
                    break;
                case "k":
                    e.preventDefault();
                    handleLink();
                    break;
                case "e":
                    e.preventDefault();
                    handleCode();
                    break;
            }
        }
    };

    const renderFormattedText = (text: string) => {
        if (!text) return <Typography color="text.secondary">{placeholder}</Typography>;

        // Split by lines to process line-based formatting
        const lines = text.split("\n");
        const elements: React.ReactElement[] = [];
        let inTable = false;
        let tableRows: string[] = [];

        const processInlineFormatting = (line: string): string => {
            let formatted = line;

            // Bold: **text**
            formatted = formatted.replace(/\*\*(.*?)\*\*/g, '<strong>$1</strong>');

            // Italic: *text* (but not inside **)
            formatted = formatted.replace(/(?<!\*)\*(?!\*)([^*]+?)(?<!\*)\*(?!\*)/g, '<em>$1</em>');

            // Code: `text`
            formatted = formatted.replace(/`([^`]+)`/g, '<code style="background-color: rgba(175, 184, 193, 0.2); padding: 2px 4px; border-radius: 3px; font-family: monospace; font-size: 0.875em;">$1</code>');

            // Images: ![alt](url) — must be before links
            formatted = formatted.replace(/!\[([^\]]*)\]\(([^)]*)\)/g, (_, alt, src) => {
                if (!src) return `<em>[uploading image…]</em>`;
                const imgSrc = normalizeAttachmentUrl(src);
                return `<img src="${imgSrc}" alt="${alt || 'image'}" style="max-width:100%;max-height:300px;border-radius:4px;display:block;margin:4px 0;cursor:pointer;" data-img-src="${imgSrc}" />`;
            });

            // Links: [text](url)
            formatted = formatted.replace(/\[([^\]]+)\]\(([^)]+)\)/g, '<a href="$2" target="_blank" rel="noopener noreferrer" style="color: #2196f3; text-decoration: none;">$1</a>');

            // Auto-detect bare URLs — skip URLs already inside src="", href="" or single-quoted JS attributes
            formatted = formatted.replace(/(?<!href=")(?<!src=")(?<!')(?<!<a[^>]*>)(https?:\/\/[^\s<"']+)/g, '<a href="$1" target="_blank" rel="noopener noreferrer" style="color: #2196f3; text-decoration: none;">$1</a>');

            // Mentions: @"Full Name" (with spaces) or @word — render as outlined chip matching CommentSection
            formatted = formatted.replace(/@"([^"]+)"|@([\w\u00C0-\u024F\u1EA0-\u1EF9]+)/g, (_, quotedName, simpleName) => {
                const name = quotedName || simpleName;
                return `<span style="display:inline-flex;align-items:center;border:1px solid rgba(33,150,243,0.7);color:#42a5f5;border-radius:16px;padding:1px 8px;font-size:0.75rem;font-weight:600;cursor:default;vertical-align:middle;margin:0 2px;white-space:nowrap;">@${name}</span>`;
            });

            return formatted;
        };

        lines.forEach((line, index) => {
            // Image-only line: ![alt](url) — render as React img to avoid URL-regex corruption
            const imageOnlyMatch = line.trim().match(/^!\[([^\]]*)\]\(([^)]*)\)$/);
            if (imageOnlyMatch) {
                const [, alt, src] = imageOnlyMatch;
                if (!src) {
                    elements.push(
                        <Typography key={index} variant="body2" color="text.secondary" sx={{ fontStyle: 'italic' }}>
                            [uploading image…]
                        </Typography>
                    );
                } else {
                    const imgSrc = normalizeAttachmentUrl(src);
                    elements.push(
                        <Box key={index} sx={{ my: 1 }}>
                            <Box
                                component="img"
                                src={imgSrc}
                                alt={alt || 'image'}
                                sx={{
                                    maxWidth: '100%',
                                    maxHeight: 300,
                                    borderRadius: 1,
                                    display: 'block',
                                    cursor: 'pointer',
                                }}
                                onClick={() => setPreviewImg(imgSrc)}
                            />
                        </Box>
                    );
                }
                return;
            }

            // Table handling
            if (line.trim().startsWith("|") && line.trim().endsWith("|")) {
                if (!inTable) {
                    inTable = true;
                    tableRows = [];
                }
                tableRows.push(line);
                return;
            } else if (inTable) {
                // End of table, render it
                elements.push(
                    <Box
                        key={`table-${index}`}
                        component="table"
                        sx={{
                            borderCollapse: "collapse",
                            width: "100%",
                            mb: 2,
                            "& td, & th": {
                                border: "1px solid",
                                borderColor: "divider",
                                padding: "8px 12px",
                                textAlign: "left",
                            },
                            "& th": {
                                backgroundColor: "action.hover",
                                fontWeight: 600,
                            },
                        }}
                    >
                                <tbody>
                            {tableRows.map((row, rowIndex) => {
                                const cells = row
                                    .split("|")
                                    .filter((cell) => cell.trim())
                                    .map((cell) => cell.trim());
                                const isHeaderSeparator = cells.every((cell) =>
                                    /^-+$/.test(cell)
                                );
                                if (isHeaderSeparator) return null;

                                const isHeader = rowIndex === 0;
                                const CellTag = isHeader ? "th" : "td";

                                return (
                                    <tr key={rowIndex}>
                                        {cells.map((cell, cellIndex) => (
                                            <CellTag
                                                key={cellIndex}
                                                dangerouslySetInnerHTML={{
                                                    __html: DOMPurify.sanitize(processInlineFormatting(cell)),
                                                }}
                                            />
                                        ))}
                                    </tr>
                                );
                            })}
                        </tbody>
                    </Box>
                );
                inTable = false;
                tableRows = [];
            }

            // Headings: # H1, ## H2, ### H3
            const headingMatch = line.match(/^(#{1,3})\s+(.+)$/);
            if (headingMatch) {
                const level = headingMatch[1].length;
                const headingText = headingMatch[2];
                const variant = level === 1 ? "h4" : level === 2 ? "h5" : "h6";
                elements.push(
                    <Typography
                        key={index}
                        variant={variant}
                        fontWeight={600}
                        sx={{ mt: level === 1 ? 3 : 2, mb: 1 }}
                        dangerouslySetInnerHTML={{
                            __html: DOMPurify.sanitize(processInlineFormatting(headingText)),
                        }}
                    />
                );
                return;
            }

            // Task list: - [ ] or - [x]
            const taskMatch = line.match(/^-\s+\[([ x])\]\s+(.+)$/);
            if (taskMatch) {
                const isChecked = taskMatch[1] === "x";
                const taskText = taskMatch[2];
                elements.push(
                    <Box
                        key={index}
                        sx={{ display: "flex", alignItems: "center", gap: 1, mb: 0.5 }}
                    >
                        <CheckIcon
                            fontSize="small"
                            sx={{
                                color: isChecked ? "success.main" : "text.disabled",
                                opacity: isChecked ? 1 : 0.3,
                            }}
                        />
                        <Typography
                            variant="body2"
                            color="text.secondary"
                            sx={{
                                textDecoration: isChecked ? "line-through" : "none",
                                opacity: isChecked ? 0.7 : 1,
                            }}
                            dangerouslySetInnerHTML={{
                                __html: DOMPurify.sanitize(processInlineFormatting(taskText)),
                            }}
                        />
                    </Box>
                );
                return;
            }

            // Bullet list: • item
            if (line.trim().startsWith("•") || line.trim().match(/^[\-\*]\s/)) {
                const bulletText = line.trim().replace(/^[•\-\*]\s/, "");
                elements.push(
                    <Box key={index} sx={{ display: 'flex', gap: 1, mb: 0.5, ml: 2 }}>
                        <Typography variant="body2" color="text.secondary">
                            •
                        </Typography>
                        <Typography
                            variant="body2"
                            color="text.secondary"
                            dangerouslySetInnerHTML={{
                                __html: DOMPurify.sanitize(processInlineFormatting(bulletText)),
                            }}
                        />
                    </Box>
                );
                return;
            }

            // Numbered list: 1. item
            const numberedMatch = line.trim().match(/^(\d+)\.\s+(.+)$/);
            if (numberedMatch) {
                const number = numberedMatch[1];
                const itemText = numberedMatch[2];
                elements.push(
                    <Box key={index} sx={{ display: 'flex', gap: 1, mb: 0.5, ml: 2 }}>
                        <Typography variant="body2" color="text.secondary" sx={{ minWidth: 20 }}>
                            {number}.
                        </Typography>
                        <Typography
                            variant="body2"
                            color="text.secondary"
                            dangerouslySetInnerHTML={{
                                __html: DOMPurify.sanitize(processInlineFormatting(itemText)),
                            }}
                        />
                    </Box>
                );
                return;
            }

            // Regular text
            if (line.trim()) {
                elements.push(
                    <Typography
                        key={index}
                        variant="body2"
                        color="text.secondary"
                        sx={{ mb: 1 }}
                        dangerouslySetInnerHTML={{
                            __html: DOMPurify.sanitize(processInlineFormatting(line)),
                        }}
                    />
                );
            } else {
                elements.push(<Box key={index} sx={{ height: 8 }} />);
            }
        });

        return <>{elements}</>;
    };

    if (!isEditing) {
        return (
            <>
                <Card sx={{ p: 2, mb: 3 }} className={className}>
                    <Box sx={{ display: "flex", alignItems: "center", justifyContent: "space-between", mb: 1 }}>
                        <Typography variant="h6" fontWeight={600}>
                            {title}
                        </Typography>
                        {!readOnly && (
                            <Tooltip title="Edit description">
                                <IconButton size="small" onClick={handleEdit}>
                                    <EditIcon fontSize="small" />
                                </IconButton>
                            </Tooltip>
                        )}
                    </Box>
                    <Box sx={{ mt: 1 }} onClick={(e: React.MouseEvent<HTMLDivElement>) => {
                        const target = e.target as HTMLElement;
                        if (target.tagName === 'IMG') {
                            const src = target.getAttribute('data-img-src') || (target as HTMLImageElement).src;
                            if (src) setPreviewImg(src);
                        }
                    }}>
                        {renderFormattedText(initialDescription)}
                    </Box>
                </Card>

                {/* Image Preview Dialog */}
                <Dialog open={Boolean(previewImg)} onClose={() => setPreviewImg(null)} maxWidth="lg">
                    <DialogContent sx={{ p: 0, lineHeight: 0 }}>
                        <Box sx={{ position: 'relative' }}>
                            <IconButton
                                size="small"
                                onClick={() => setPreviewImg(null)}
                                sx={{ position: 'absolute', top: 8, right: 8, bgcolor: 'rgba(0,0,0,0.5)', color: 'white', '&:hover': { bgcolor: 'rgba(0,0,0,0.7)' }, zIndex: 1 }}
                            >
                                <CloseIcon fontSize="small" />
                            </IconButton>
                            {previewImg && (
                                <Box
                                    component="img"
                                    src={previewImg}
                                    alt="Preview"
                                    sx={{ maxWidth: '90vw', maxHeight: '85vh', display: 'block', objectFit: 'contain' }}
                                />
                            )}
                        </Box>
                    </DialogContent>
                </Dialog>
            </>
        );
    }

    const editorCard = (
        <Card
            sx={{
                p: 2,
                mb: 3,
                ...(isFullscreen && {
                    position: "fixed",
                    top: 0,
                    left: 0,
                    right: 0,
                    bottom: 0,
                    zIndex: 1300,
                    m: 0,
                    borderRadius: 0,
                    display: "flex",
                    flexDirection: "column",
                    overflow: "hidden",
                })
            }}
            className={className}
        >
            <Box sx={{ display: "flex", alignItems: "center", justifyContent: "space-between" }}>
                <Typography variant="h6" fontWeight={600} gutterBottom>
                    {title}
                </Typography>
                <Tooltip title={isFullscreen ? "Exit fullscreen" : "Fullscreen"}>
                    <IconButton size="small" onClick={() => setIsFullscreen(!isFullscreen)}>
                        {isFullscreen ? <FullscreenExitIcon fontSize="small" /> : <FullscreenIcon fontSize="small" />}
                    </IconButton>
                </Tooltip>
            </Box>

            <Box sx={{ borderBottom: 1, borderColor: "divider", mb: 1 }}>
                <Tabs
                    value={activeTab}
                    onChange={(_, newValue) => setActiveTab(newValue)}
                    sx={{ minHeight: 40 }}
                >
                    <Tab label="Write" value="write" sx={{ minHeight: 40, py: 1 }} />
                    <Tab label="Preview" value="preview" sx={{ minHeight: 40, py: 1 }} />
                </Tabs>
            </Box>

            <Box sx={{ flex: isFullscreen ? 1 : "unset", overflow: "auto", display: "flex", flexDirection: "column" }}>
                {activeTab === "write" && (
                    <Box sx={{ flex: 1, display: "flex", flexDirection: "column" }}>
                        {/* Toolbar */}
                        <Box
                            sx={{
                                display: "flex",
                                gap: 0.5,
                                mb: 1,
                                pb: 1,
                                borderBottom: 1,
                                borderColor: "divider",
                                flexWrap: "wrap",
                            }}
                        >
                            {/* Text Formatting */}
                            <Tooltip title="Bold (Ctrl+B)">
                                <IconButton size="small" onClick={handleBold}>
                                    <BoldIcon fontSize="small" />
                                </IconButton>
                            </Tooltip>
                            <Tooltip title="Italic (Ctrl+I)">
                                <IconButton size="small" onClick={handleItalic}>
                                    <ItalicIcon fontSize="small" />
                                </IconButton>
                            </Tooltip>
                            <Tooltip title="Code (Ctrl+E)">
                                <IconButton size="small" onClick={handleCode}>
                                    <CodeIcon fontSize="small" />
                                </IconButton>
                            </Tooltip>

                            <Divider orientation="vertical" flexItem sx={{ mx: 0.5 }} />

                            {/* Headings */}
                            <Tooltip title="Heading 1">
                                <IconButton size="small" onClick={() => handleHeading(1)}>
                                    <Typography fontSize="small" fontWeight={700}>H1</Typography>
                                </IconButton>
                            </Tooltip>
                            <Tooltip title="Heading 2">
                                <IconButton size="small" onClick={() => handleHeading(2)}>
                                    <Typography fontSize="small" fontWeight={600}>H2</Typography>
                                </IconButton>
                            </Tooltip>
                            <Tooltip title="Heading 3">
                                <IconButton size="small" onClick={() => handleHeading(3)}>
                                    <Typography fontSize="small" fontWeight={500}>H3</Typography>
                                </IconButton>
                            </Tooltip>

                            <Divider orientation="vertical" flexItem sx={{ mx: 0.5 }} />

                            {/* Lists */}
                            <Tooltip title="Bullet list">
                                <IconButton size="small" onClick={handleBulletList}>
                                    <BulletIcon fontSize="small" />
                                </IconButton>
                            </Tooltip>
                            <Tooltip title="Numbered list">
                                <IconButton size="small" onClick={handleNumberedList}>
                                    <NumberedIcon fontSize="small" />
                                </IconButton>
                            </Tooltip>
                            <Tooltip title="Task list">
                                <IconButton size="small" onClick={handleTaskList}>
                                    <TaskListIcon fontSize="small" />
                                </IconButton>
                            </Tooltip>

                            <Divider orientation="vertical" flexItem sx={{ mx: 0.5 }} />

                            {/* Insert elements */}
                            <Tooltip title="Link (Ctrl+K)">
                                <IconButton size="small" onClick={handleLink}>
                                    <LinkIcon fontSize="small" />
                                </IconButton>
                            </Tooltip>
                            <Tooltip title="Table">
                                <IconButton size="small" onClick={handleTable}>
                                    <TableIcon fontSize="small" />
                                </IconButton>
                            </Tooltip>
                            {members.length > 0 && (
                                <Tooltip title="Mention (@)">
                                    <IconButton
                                        size="small"
                                        onClick={() => {
                                            const textarea = textFieldRef.current;
                                            if (textarea) {
                                                const cursorPos = textarea.selectionStart;
                                                const newText = description.substring(0, cursorPos) + "@" + description.substring(cursorPos);
                                                setDescription(newText);
                                                setTimeout(() => {
                                                    textarea.focus();
                                                    textarea.setSelectionRange(cursorPos + 1, cursorPos + 1);
                                                }, 0);
                                            }
                                        }}
                                    >
                                        <MentionIcon fontSize="small" />
                                    </IconButton>
                                </Tooltip>
                            )}
                            <Tooltip title="Upload image">
                                <IconButton size="small" onClick={() => fileInputRef.current?.click()} disabled={isUploadingImage}>
                                    <ImageIcon fontSize="small" />
                                </IconButton>
                            </Tooltip>
                            <input
                                ref={fileInputRef}
                                type="file"
                                accept="image/*"
                                multiple
                                style={{ display: 'none' }}
                                onChange={handleImageFileSelect}
                            />
                            {isUploadingImage && (
                                <Box sx={{ display: "flex", alignItems: "center", gap: 0.5, ml: 1 }}>
                                    <CircularProgress size={14} />
                                    <Typography variant="caption" color="text.secondary">Uploading image…</Typography>
                                </Box>
                            )}
                        </Box>

                        {/* Text Field */}
                        <TextField
                            fullWidth
                            multiline
                            minRows={isFullscreen ? 20 : 6}
                            maxRows={isFullscreen ? undefined : 20}
                            value={description}
                            onChange={handleTextChange}
                            onKeyDown={handleKeyDown}
                            onPaste={handlePaste}
                            placeholder={isUploadingImage ? "Uploading image..." : placeholder}
                            inputRef={textFieldRef}
                            variant="outlined"
                            sx={{
                                flex: isFullscreen ? 1 : "unset",
                                "& .MuiOutlinedInput-root": {
                                    fontFamily: "monospace",
                                    fontSize: "0.875rem",
                                    height: isFullscreen ? "100%" : "auto",
                                },
                                "& .MuiInputBase-inputMultiline": {
                                    height: isFullscreen ? "100% !important" : "auto",
                                },
                            }}
                        />

                        {/* Mention Autocomplete */}
                        {mentionAnchor && filteredMembers.length > 0 && (
                            <Popper
                                open={Boolean(mentionAnchor)}
                                anchorEl={mentionAnchor.element}
                                placement="bottom-start"
                                sx={{ zIndex: 1400 }}
                            >
                                <ClickAwayListener onClickAway={() => setMentionAnchor(null)}>
                                    <Paper
                                        elevation={8}
                                        sx={{
                                            mt: 1,
                                            maxHeight: 200,
                                            overflow: "auto",
                                            minWidth: 200,
                                        }}
                                    >
                                        <List dense>
                                            {filteredMembers.slice(0, 10).map((member, idx) => (
                                                <ListItemButton
                                                    key={member.memberId}
                                                    selected={idx === selectedMentionIndex}
                                                    onClick={() => insertMention(member)}
                                                >
                                                    <ListItemAvatar>
                                                        <Avatar
                                                            src={member.memberImage}
                                                            alt={member.memberName}
                                                            sx={{ width: 24, height: 24 }}
                                                        >
                                                            {member.memberName[0]}
                                                        </Avatar>
                                                    </ListItemAvatar>
                                                    <ListItemText
                                                        primary={member.memberName}
                                                        primaryTypographyProps={{ variant: "body2" }}
                                                    />
                                                </ListItemButton>
                                            ))}
                                        </List>
                                    </Paper>
                                </ClickAwayListener>
                            </Popper>
                        )}
                    </Box>
                )}

                {activeTab === "preview" && (
                    <Box
                        sx={{
                            minHeight: isFullscreen ? 400 : 200,
                            flex: isFullscreen ? 1 : "unset",
                            p: 2,
                            border: 1,
                            borderColor: "divider",
                            borderRadius: 1,
                            bgcolor: "background.paper",
                            overflow: "auto",
                        }}
                        onClick={(e: React.MouseEvent<HTMLDivElement>) => {
                            const target = e.target as HTMLElement;
                            if (target.tagName === 'IMG') {
                                const src = target.getAttribute('data-img-src') || (target as HTMLImageElement).src;
                                if (src) setPreviewImg(src);
                            }
                        }}
                    >
                        {renderFormattedText(description)}
                    </Box>
                )}
            </Box>

            <Box sx={{ display: "flex", gap: 1, mt: 2 }}>
                <Button
                    variant="contained"
                    color="primary"
                    startIcon={isSaving ? <CircularProgress size={16} color="inherit" /> : <CheckIcon />}
                    onClick={handleSave}
                    disabled={isSaving || isUploadingImage}
                >
                    {isSaving ? "Saving..." : isUploadingImage ? "Uploading image..." : "Save"}
                </Button>
                <Button
                    variant="outlined"
                    color="inherit"
                    startIcon={<CloseIcon />}
                    onClick={handleCancel}
                    disabled={isSaving}
                >
                    Cancel
                </Button>
            </Box>
        </Card>
    );

    return (
        <>
            {editorCard}
            {/* Image Preview Dialog */}
            <Dialog open={Boolean(previewImg)} onClose={() => setPreviewImg(null)} maxWidth="lg">
                <DialogContent sx={{ p: 0, lineHeight: 0 }}>
                    <Box sx={{ position: 'relative' }}>
                        <IconButton
                            size="small"
                            onClick={() => setPreviewImg(null)}
                            sx={{ position: 'absolute', top: 8, right: 8, bgcolor: 'rgba(0,0,0,0.5)', color: 'white', '&:hover': { bgcolor: 'rgba(0,0,0,0.7)' }, zIndex: 1 }}
                        >
                            <CloseIcon fontSize="small" />
                        </IconButton>
                        {previewImg && (
                            <Box
                                component="img"
                                src={previewImg}
                                alt="Preview"
                                sx={{ maxWidth: '90vw', maxHeight: '85vh', display: 'block', objectFit: 'contain' }}
                            />
                        )}
                    </Box>
                </DialogContent>
            </Dialog>
        </>
    );
};
