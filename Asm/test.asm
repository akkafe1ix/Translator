Ожидалось Delimiter, а получили Begin (Строка:3, позиция:0, символ:'
')
Недопустимое выражение. (Строка 5, позиция 0, символ '10')
Несовместимые типы для операции сравнения.
Ожидалось While, а получили EndWhile (Строка:7, позиция:0, символ:'
')
Ожидалось End, а получили EndWhile (Строка:7, позиция:0, символ:'
')
data segment
a  dw    1
b  dw    1
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
mov a, ax
lable1:
mov ax, a
push ax
mov ax, 6
push ax
pop ax
pop bx
cmp bx, ax
jge lable2
mov ax, 1
push ax
pop ax
mov a, ax
jmp lable1
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
