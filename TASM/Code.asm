data segment
a  dw    0
b  dw    0
c  dw    0
g  dw    0
z  dw    0
v  dw    0
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
mov a, ax
mov ax, 34
push ax
pop ax
mov b, ax
mov ax, 0
push ax
pop ax
mov c, ax
mov ax, 0
push ax
pop ax
mov g, ax
mov ax, 1
push ax
pop ax
mov z, ax
mov ax, 1
push ax
pop ax
mov v, ax
label1:
mov ax, v
push ax
mov ax, 1
push ax
mov ax, 0
push ax
pop ax
pop bx
or ax, bx
push ax
cmp ax, 0
jnz label3
jmp label2
label3:
pop ax
pop bx
and ax, bx
push ax
cmp ax, 0
jz label2
pop ax
not ax
and ax, 1
push ax
mov ax, z
push ax
pop ax
pop bx
xor ax, bx
push ax
cmp ax, 1
jnz label2
jmp label4
label4:
mov ax, a
push ax
mov ax, b
push ax
mov ax, 2
push ax
pop bx
pop ax
mul bx
push ax
pop ax
pop bx
cmp bx, ax
jle label5
mov ax, 0
push ax
pop ax
mov v, ax
jmp label6
label5:
mov ax, 1
push ax
pop ax
mov v, ax
label6:
label7:
mov ax, a
push ax
mov ax, 75
push ax
pop ax
pop bx
cmp bx, ax
jge label8
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
jmp label7
label8:
mov ax, c
push ax
mov ax, 1
push ax
pop bx
pop ax
add ax, bx
push ax
pop ax
mov c, ax
jmp label1
label2:
push ax
mov ax, a
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
