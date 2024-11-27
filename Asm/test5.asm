data segment
a  dw    1
b  dw    1
c  dw    1
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
mov ax, 7
push ax
pop ax
mov a, ax
mov ax, 11
push ax
pop ax
mov b, ax
mov ax, b
push ax
mov ax, 10
push ax
pop ax
pop bx
cmp bx, ax
jle lable1
push ax
mov ax, b
CALL PRINT
pop ax
jmp lable2
lable1:
mov ax, b
push ax
mov ax, 10
push ax
pop ax
pop bx
cmp bx, ax
jge lable3
push ax
mov ax, b
CALL PRINT
pop ax
jmp lable2
lable3:
push ax
mov ax, a
CALL PRINT
pop ax
lable2:
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
