/**
 * HtmlContentEditor — A lightweight HTML content editor with formatting toolbar.
 *
 * Toolbar actions insert/wrap raw HTML tags (not Markdown).
 * Preview tab renders the HTML via dangerouslySetInnerHTML.
 * No image upload, no @mention, no Markdown.
 * Undo/Redo supported (Ctrl+Z / Ctrl+Y).
 */
import React, { useState, useRef, useCallback } from 'react';
import DOMPurify from 'dompurify';
import {
    Box, Button, Divider, IconButton, Paper, Tab, Tabs,
    TextField, Tooltip, Typography, CircularProgress,
} from '@mui/material';
import {
    Edit as EditIcon,
    Close as CloseIcon,
    Check as CheckIcon,
    Fullscreen as FullscreenIcon,
    FullscreenExit as FullscreenExitIcon,
    FormatBold as BoldIcon,
    FormatItalic as ItalicIcon,
    FormatUnderlined as UnderlineIcon,
    StrikethroughS as StrikeIcon,
    FormatListBulleted as BulletIcon,
    FormatListNumbered as NumberedIcon,
    FormatIndentIncrease as IndentIcon,
    FormatIndentDecrease as OutdentIcon,
    Link as LinkIcon,
    HorizontalRule as HrIcon,
    Code as CodeIcon,
    Undo as UndoIcon,
    Redo as RedoIcon,
    TableChart as TableIcon,
} from '@mui/icons-material';
import { logError } from '../utils/errorHandler';

interface HtmlContentEditorProps {
    /** Initial HTML content */
    initialContent: string;
    /** Label shown above the editor */
    title?: string;
    /** Placeholder when empty */
    placeholder?: string;
    /** Called when user clicks Save */
    onSave: (newContent: string) => Promise<void>;
    /** Read-only mode — shows preview only, no edit button */
    readOnly?: boolean;
}

export const HtmlContentEditor: React.FC<HtmlContentEditorProps> = ({
    initialContent,
    title = 'Content (HTML)',
    placeholder = 'Enter HTML content…',
    onSave,
    readOnly = false,
}) => {
    const [isEditing, setIsEditing] = useState(false);
    const [content, setContent] = useState(initialContent);
    const [activeTab, setActiveTab] = useState<'write' | 'preview'>('write');
    const [isSaving, setIsSaving] = useState(false);
    const [isFullscreen, setIsFullscreen] = useState(false);

    const textFieldRef = useRef<HTMLTextAreaElement>(null);

    // ─── Undo / Redo ──────────────────────────────────────────────────────────
    const undoStackRef = useRef<string[]>([]);
    const redoStackRef = useRef<string[]>([]);
    const burstStartRef = useRef<string | null>(null);
    const burstTimerRef = useRef<ReturnType<typeof setTimeout> | null>(null);

    const setContentWithUndo = useCallback((newValue: string, currentContent: string) => {
        undoStackRef.current.push(currentContent);
        if (undoStackRef.current.length > 100) undoStackRef.current.shift();
        redoStackRef.current = [];
        if (burstTimerRef.current) { clearTimeout(burstTimerRef.current); burstTimerRef.current = null; }
        burstStartRef.current = null;
        setContent(newValue);
    }, []);

    // ─── Keyboard shortcuts ───────────────────────────────────────────────────
    const handleKeyDown = useCallback((e: React.KeyboardEvent) => {
        const ctrl = e.ctrlKey || e.metaKey;
        if (!ctrl) return;
        if (e.key === 'z' && !e.shiftKey) {
            e.preventDefault();
            setContent(cur => {
                if (undoStackRef.current.length > 0) {
                    const prev = undoStackRef.current.pop()!;
                    redoStackRef.current.push(cur);
                    return prev;
                }
                return cur;
            });
            return;
        }
        if (e.key === 'y' || (e.key === 'z' && e.shiftKey)) {
            e.preventDefault();
            setContent(cur => {
                if (redoStackRef.current.length > 0) {
                    const next = redoStackRef.current.pop()!;
                    undoStackRef.current.push(cur);
                    return next;
                }
                return cur;
            });
        }
    }, []);

    // ─── Typing burst → undo snapshot every 1.5 s of inactivity ─────────────
    const handleChange = useCallback((e: React.ChangeEvent<HTMLTextAreaElement>) => {
        const newValue = e.target.value;
        setContent(cur => {
            if (burstStartRef.current === null) burstStartRef.current = cur;
            return newValue;
        });
        if (burstTimerRef.current) clearTimeout(burstTimerRef.current);
        burstTimerRef.current = setTimeout(() => {
            if (burstStartRef.current !== null) {
                undoStackRef.current.push(burstStartRef.current);
                if (undoStackRef.current.length > 100) undoStackRef.current.shift();
                redoStackRef.current = [];
                burstStartRef.current = null;
            }
            burstTimerRef.current = null;
        }, 1500);
    }, []);

    // ─── Core insert helpers ──────────────────────────────────────────────────
    // These read content via a ref-snapshot captured at call time
    const insertWrap = useCallback((openTag: string, closeTag: string, ph: string) => {
        const ta = textFieldRef.current;
        if (!ta) return;
        const snap = ta.value; // always in sync with state
        const start = ta.selectionStart;
        const end = ta.selectionEnd;
        const inner = snap.substring(start, end) || ph;
        const newText = snap.substring(0, start) + openTag + inner + closeTag + snap.substring(end);
        setContentWithUndo(newText, snap);
        setTimeout(() => {
            ta.focus();
            const cursor = start + openTag.length + inner.length + closeTag.length;
            ta.setSelectionRange(cursor, cursor);
        }, 0);
    }, [setContentWithUndo]);

    const insertAt = useCallback((snippet: string) => {
        const ta = textFieldRef.current;
        if (!ta) return;
        const snap = ta.value;
        const pos = ta.selectionStart;
        const newText = snap.substring(0, pos) + snippet + snap.substring(pos);
        setContentWithUndo(newText, snap);
        setTimeout(() => { ta.focus(); ta.setSelectionRange(pos + snippet.length, pos + snippet.length); }, 0);
    }, [setContentWithUndo]);

    // ─── Toolbar handlers ─────────────────────────────────────────────────────
    const handleHeading = useCallback((level: 1 | 2 | 3) => {
        const ta = textFieldRef.current;
        if (!ta) return;
        const snap = ta.value;
        const start = ta.selectionStart;
        const lineStart = snap.lastIndexOf('\n', start - 1) + 1;
        const lineEndIdx = snap.indexOf('\n', start);
        const lineEnd = lineEndIdx === -1 ? snap.length : lineEndIdx;
        const line = snap.substring(lineStart, lineEnd);
        const tag = `h${level}`;
        const existing = line.match(new RegExp(`^<h${level}>(.*)<\\/h${level}>$`));
        const newLine = existing
            ? existing[1]
            : `<${tag}>${line.replace(/<\/?h[1-6]>/g, '')}</${tag}>`;
        const newText = snap.substring(0, lineStart) + newLine + snap.substring(lineEnd);
        setContentWithUndo(newText, snap);
        setTimeout(() => ta.focus(), 0);
    }, [setContentWithUndo]);

    const handleBulletList = useCallback(() => {
        const ta = textFieldRef.current;
        if (!ta) return;
        const snap = ta.value;
        const start = ta.selectionStart;
        const end = ta.selectionEnd;
        if (start === end) {
            const pos = start;
            const snippet = '\n<ul>\n  <li></li>\n</ul>\n';
            const newText = snap.substring(0, pos) + snippet + snap.substring(pos);
            setContentWithUndo(newText, snap);
            setTimeout(() => { ta.focus(); ta.setSelectionRange(pos + snippet.length, pos + snippet.length); }, 0);
        } else {
            const selected = snap.substring(start, end);
            const items = selected.split('\n').filter(l => l.trim()).map(l => `  <li>${l.trim()}</li>`).join('\n');
            const replacement = `<ul>\n${items}\n</ul>`;
            setContentWithUndo(snap.substring(0, start) + replacement + snap.substring(end), snap);
            setTimeout(() => ta.focus(), 0);
        }
    }, [setContentWithUndo]);

    const handleNumberedList = useCallback(() => {
        const ta = textFieldRef.current;
        if (!ta) return;
        const snap = ta.value;
        const start = ta.selectionStart;
        const end = ta.selectionEnd;
        if (start === end) {
            const snippet = '\n<ol>\n  <li></li>\n</ol>\n';
            const newText = snap.substring(0, start) + snippet + snap.substring(start);
            setContentWithUndo(newText, snap);
            setTimeout(() => { ta.focus(); ta.setSelectionRange(start + snippet.length, start + snippet.length); }, 0);
        } else {
            const selected = snap.substring(start, end);
            const items = selected.split('\n').filter(l => l.trim()).map(l => `  <li>${l.trim()}</li>`).join('\n');
            const replacement = `<ol>\n${items}\n</ol>`;
            setContentWithUndo(snap.substring(0, start) + replacement + snap.substring(end), snap);
            setTimeout(() => ta.focus(), 0);
        }
    }, [setContentWithUndo]);

    const handleIndent = useCallback(() => {
        insertWrap(
            '<blockquote style="margin-left:2em;border-left:3px solid #ccc;padding-left:0.8em;color:#555">',
            '</blockquote>',
            'indented text'
        );
    }, [insertWrap]);

    const handleOutdent = useCallback(() => {
        const ta = textFieldRef.current;
        if (!ta) return;
        const snap = ta.value;
        const start = ta.selectionStart;
        const end = ta.selectionEnd;
        const selected = snap.substring(start, end);
        const stripped = selected.replace(/<blockquote[^>]*>([\s\S]*?)<\/blockquote>/g, '$1');
        if (stripped !== selected) {
            setContentWithUndo(snap.substring(0, start) + stripped + snap.substring(end), snap);
            setTimeout(() => ta.focus(), 0);
        }
    }, [setContentWithUndo]);

    const handleLink = useCallback(() => {
        const ta = textFieldRef.current;
        if (!ta) return;
        const snap = ta.value;
        const start = ta.selectionStart;
        const end = ta.selectionEnd;
        const linkText = snap.substring(start, end) || 'link text';
        const snippet = `<a href="https://" target="_blank">${linkText}</a>`;
        setContentWithUndo(snap.substring(0, start) + snippet + snap.substring(end), snap);
        setTimeout(() => {
            ta.focus();
            ta.setSelectionRange(start + 9, start + 17); // select "https://"
        }, 0);
    }, [setContentWithUndo]);

    const handleHr = useCallback(() => insertAt('\n<hr />\n'), [insertAt]);

    const handleTable = useCallback(() => insertAt(
        '\n<table border="1" cellpadding="6" style="border-collapse:collapse;width:100%">\n' +
        '  <thead>\n    <tr><th>Header 1</th><th>Header 2</th><th>Header 3</th></tr>\n  </thead>\n' +
        '  <tbody>\n    <tr><td>Cell 1</td><td>Cell 2</td><td>Cell 3</td></tr>\n' +
        '    <tr><td>Cell 4</td><td>Cell 5</td><td>Cell 6</td></tr>\n  </tbody>\n</table>\n'
    ), [insertAt]);

    const handleUndoClick = useCallback(() => {
        setContent(cur => {
            if (undoStackRef.current.length > 0) {
                const prev = undoStackRef.current.pop()!;
                redoStackRef.current.push(cur);
                return prev;
            }
            return cur;
        });
    }, []);

    const handleRedoClick = useCallback(() => {
        setContent(cur => {
            if (redoStackRef.current.length > 0) {
                const next = redoStackRef.current.pop()!;
                undoStackRef.current.push(cur);
                return next;
            }
            return cur;
        });
    }, []);

    // ─── Edit / Cancel / Save ─────────────────────────────────────────────────
    const handleEdit = () => {
        setContent(initialContent);
        setIsEditing(true);
        setActiveTab('write');
        undoStackRef.current = [];
        redoStackRef.current = [];
        burstStartRef.current = null;
        if (burstTimerRef.current) clearTimeout(burstTimerRef.current);
    };

    const handleCancel = () => {
        setContent(initialContent);
        setIsEditing(false);
        setActiveTab('write');
        setIsFullscreen(false);
        if (burstTimerRef.current) clearTimeout(burstTimerRef.current);
    };

    const handleSave = async () => {
        if (content.trim() === initialContent.trim()) {
            setIsEditing(false);
            setIsFullscreen(false);
            return;
        }
        setIsSaving(true);
        try {
            await onSave(content.trim());
            setIsEditing(false);
            setIsFullscreen(false);
        } catch (error) {
            logError('HtmlContentEditor.handleSave', error);
        } finally {
            setIsSaving(false);
        }
    };

    // ─── Layout ───────────────────────────────────────────────────────────────
    const editorHeight = isFullscreen ? 'calc(100vh - 230px)' : 340;

    const containerSx = isFullscreen
        ? {
            position: 'fixed' as const,
            inset: 0,
            zIndex: 1400,
            bgcolor: 'background.paper',
            display: 'flex',
            flexDirection: 'column' as const,
            p: 2,
            overflow: 'hidden',
        }
        : {};

    // ─── Read-only ────────────────────────────────────────────────────────────
    if (!isEditing) {
        return (
            <Box>
                {title && (
                    <Box display="flex" alignItems="center" justifyContent="space-between" mb={1}>
                        <Typography variant="subtitle2" color="text.secondary">{title}</Typography>
                        {!readOnly && (
                            <Button size="small" startIcon={<EditIcon />} onClick={handleEdit}>Edit</Button>
                        )}
                    </Box>
                )}
                <Paper variant="outlined" sx={{ p: 2, minHeight: 80, maxHeight: 400, overflow: 'auto', bgcolor: '#fafafa' }}>
                    {initialContent ? (
                        <div dangerouslySetInnerHTML={{ __html: DOMPurify.sanitize(initialContent) }} style={{ fontSize: 13, lineHeight: 1.7 }} />
                    ) : (
                        <Typography variant="body2" color="text.disabled" fontStyle="italic">{placeholder}</Typography>
                    )}
                </Paper>
            </Box>
        );
    }

    // ─── Edit mode ────────────────────────────────────────────────────────────
    return (
        <Box sx={containerSx} onKeyDown={handleKeyDown}>
            {/* Header */}
            <Box display="flex" alignItems="center" justifyContent="space-between" mb={0.5}>
                {title ? <Typography variant="subtitle2" color="text.secondary">{title}</Typography> : <Box />}
                <Tooltip title={isFullscreen ? 'Exit fullscreen' : 'Fullscreen'}>
                    <IconButton size="small" onClick={() => setIsFullscreen(f => !f)}>
                        {isFullscreen ? <FullscreenExitIcon fontSize="small" /> : <FullscreenIcon fontSize="small" />}
                    </IconButton>
                </Tooltip>
            </Box>

            {/* Tabs */}
            <Tabs
                value={activeTab}
                onChange={(_, v) => setActiveTab(v)}
                sx={{ borderBottom: 1, borderColor: 'divider', mb: 0.75, minHeight: 34 }}
                TabIndicatorProps={{ style: { height: 2 } }}
            >
                <Tab label="Write" value="write" sx={{ minHeight: 34, py: 0.5, textTransform: 'none', fontSize: 13 }} />
                <Tab label="Preview" value="preview" sx={{ minHeight: 34, py: 0.5, textTransform: 'none', fontSize: 13 }} />
            </Tabs>

            {/* Toolbar */}
            {activeTab === 'write' && (
                <Box sx={{
                    display: 'flex', flexWrap: 'wrap', alignItems: 'center',
                    gap: 0.25, mb: 0.75, px: 0.5, py: 0.5,
                    bgcolor: 'grey.50', border: '1px solid', borderColor: 'divider', borderRadius: 1,
                }}>
                    {/* Undo / Redo */}
                    <Tooltip title="Undo (Ctrl+Z)"><IconButton size="small" onClick={handleUndoClick}><UndoIcon sx={{ fontSize: 17 }} /></IconButton></Tooltip>
                    <Tooltip title="Redo (Ctrl+Y)"><IconButton size="small" onClick={handleRedoClick}><RedoIcon sx={{ fontSize: 17 }} /></IconButton></Tooltip>

                    <Divider orientation="vertical" flexItem sx={{ mx: 0.5, my: 0.25 }} />

                    {/* Text formatting */}
                    <Tooltip title="Bold (Ctrl+B)"><IconButton size="small" onClick={() => insertWrap('<strong>', '</strong>', 'bold text')}><BoldIcon sx={{ fontSize: 17 }} /></IconButton></Tooltip>
                    <Tooltip title="Italic (Ctrl+I)"><IconButton size="small" onClick={() => insertWrap('<em>', '</em>', 'italic text')}><ItalicIcon sx={{ fontSize: 17 }} /></IconButton></Tooltip>
                    <Tooltip title="Underline (Ctrl+U)"><IconButton size="small" onClick={() => insertWrap('<u>', '</u>', 'underlined text')}><UnderlineIcon sx={{ fontSize: 17 }} /></IconButton></Tooltip>
                    <Tooltip title="Strikethrough"><IconButton size="small" onClick={() => insertWrap('<s>', '</s>', 'strikethrough')}><StrikeIcon sx={{ fontSize: 17 }} /></IconButton></Tooltip>
                    <Tooltip title="Inline code"><IconButton size="small" onClick={() => insertWrap('<code style="background:#f0f0f0;padding:1px 4px;border-radius:3px;font-family:monospace">', '</code>', 'code')}><CodeIcon sx={{ fontSize: 17 }} /></IconButton></Tooltip>

                    <Divider orientation="vertical" flexItem sx={{ mx: 0.5, my: 0.25 }} />

                    {/* Headings */}
                    <Tooltip title="Heading 1"><IconButton size="small" onClick={() => handleHeading(1)}><Typography sx={{ fontSize: 11, fontWeight: 700, lineHeight: 1, userSelect: 'none' }}>H1</Typography></IconButton></Tooltip>
                    <Tooltip title="Heading 2"><IconButton size="small" onClick={() => handleHeading(2)}><Typography sx={{ fontSize: 11, fontWeight: 600, lineHeight: 1, userSelect: 'none' }}>H2</Typography></IconButton></Tooltip>
                    <Tooltip title="Heading 3"><IconButton size="small" onClick={() => handleHeading(3)}><Typography sx={{ fontSize: 11, fontWeight: 500, lineHeight: 1, userSelect: 'none' }}>H3</Typography></IconButton></Tooltip>

                    <Divider orientation="vertical" flexItem sx={{ mx: 0.5, my: 0.25 }} />

                    {/* Lists */}
                    <Tooltip title="Bullet list"><IconButton size="small" onClick={handleBulletList}><BulletIcon sx={{ fontSize: 17 }} /></IconButton></Tooltip>
                    <Tooltip title="Numbered list"><IconButton size="small" onClick={handleNumberedList}><NumberedIcon sx={{ fontSize: 17 }} /></IconButton></Tooltip>

                    <Divider orientation="vertical" flexItem sx={{ mx: 0.5, my: 0.25 }} />

                    {/* Indent / Outdent */}
                    <Tooltip title="Indent (blockquote)"><IconButton size="small" onClick={handleIndent}><IndentIcon sx={{ fontSize: 17 }} /></IconButton></Tooltip>
                    <Tooltip title="Outdent (remove blockquote)"><IconButton size="small" onClick={handleOutdent}><OutdentIcon sx={{ fontSize: 17 }} /></IconButton></Tooltip>

                    <Divider orientation="vertical" flexItem sx={{ mx: 0.5, my: 0.25 }} />

                    {/* Insert */}
                    <Tooltip title="Link (Ctrl+K)"><IconButton size="small" onClick={handleLink}><LinkIcon sx={{ fontSize: 17 }} /></IconButton></Tooltip>
                    <Tooltip title="Insert table"><IconButton size="small" onClick={handleTable}><TableIcon sx={{ fontSize: 17 }} /></IconButton></Tooltip>
                    <Tooltip title="Horizontal rule"><IconButton size="small" onClick={handleHr}><HrIcon sx={{ fontSize: 17 }} /></IconButton></Tooltip>
                </Box>
            )}

            {/* Editor / Preview */}
            <Box flex={1} sx={{ overflow: 'hidden' }}>
                {activeTab === 'write' ? (
                    <TextField
                        inputRef={textFieldRef}
                        multiline
                        fullWidth
                        value={content}
                        onChange={handleChange}
                        placeholder={placeholder}
                        variant="outlined"
                        inputProps={{ style: { fontFamily: 'monospace', fontSize: 12, lineHeight: 1.6 } }}
                        sx={{
                            '& .MuiInputBase-root': { height: editorHeight, alignItems: 'flex-start', overflow: 'auto' },
                            '& textarea': { resize: 'none' },
                        }}
                    />
                ) : (
                    <Paper variant="outlined" sx={{ p: 2, height: editorHeight, overflow: 'auto', bgcolor: '#fff' }}>
                        {content ? (
                            <div
                                dangerouslySetInnerHTML={{ __html: DOMPurify.sanitize(content) }}
                                style={{ fontSize: 13, lineHeight: 1.7 }}
                            />
                        ) : (
                            <Typography variant="body2" color="text.disabled" fontStyle="italic">
                                Nothing to preview.
                            </Typography>
                        )}
                    </Paper>
                )}
            </Box>

            {/* Actions */}
            <Box display="flex" justifyContent="flex-end" gap={1} mt={1.5}>
                <Button size="small" startIcon={<CloseIcon />} onClick={handleCancel} disabled={isSaving}>
                    Cancel
                </Button>
                <Button
                    size="small" variant="contained"
                    startIcon={isSaving ? <CircularProgress size={14} color="inherit" /> : <CheckIcon />}
                    onClick={handleSave}
                    disabled={isSaving}
                >
                    {isSaving ? 'Saving…' : 'Save'}
                </Button>
            </Box>
        </Box>
    );
};

export default HtmlContentEditor;
