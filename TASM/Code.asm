data segment
a  dw    0
b  dw    0
c  dw    0
g  db    0
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
mov ax, 0
push ax
pop ax
mov g, ax
mov ax, 0
push ax
pop ax
mov a, ax
label1:
mov ax, g
push ax
pop ax
cmp ax, 1
jne label2
mov ax, a
push ax
mov ax, 10
push ax
pop ax
pop bx
cmp bx, ax
jne label3
mov ax, 1
push ax
pop ax
mov g, ax
jmp label4
label3:
label4:
mov ax, a
push ax
mov ax, 1
push ax
pop bx
pop ax
add ax, bx
push ax
pop ax
mov a, ax
push ax
mov ax, a
CALL PRINT
pop ax
jmp label1
label2:
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
