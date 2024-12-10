data segment
g  dw    0
z  dw    0
v  dw    0
k  dw    0
PRINT_BUF DB ' ' DUP(10)
BUFEND    DB '$'
data ends
stk segment stack
db 256 dup ("?")
stk ends
code segment
assume cs:code,ds:data,ss:stk
main proc
mov ax,data
mov ds,ax
mov ax, 1
push ax
pop ax
mov z, ax
mov ax, 0
push ax
pop ax
mov v, ax
mov ax, z
push ax
mov ax, v
push ax
pop ax
pop bx
or ax, bx
push ax
cmp ax, 0
jnz label1
jmp 
label1:
pop ax
mov k, ax
push ax
mov ax, k
CALL PRINT
pop ax
mov ax,4c00h
int 21h
main endp
PRINT PROC NEAR
MOV   CX, 10
MOV   DI, BUFEND - PRINT_BUF
PRINT_LOOP:
MOV   DX, 0
DIV   CX
ADD   DL, '0'
MOV   [PRINT_BUF + DI - 1], DL
DEC   DI
CMP   AL, 0
JNE   PRINT_LOOP
LEA   DX, PRINT_BUF
ADD   DX, DI
MOV   AH, 09H
INT   21H
RET
PRINT ENDP
code ends
end main
