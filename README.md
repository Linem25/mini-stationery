# Mini Stationery - Secure MVC Final Project (Lab06)

Ứng dụng quản lý cửa hàng văn phòng phẩm (Mini Stationery) xây dựng bằng ASP.NET Core MVC, EF Core, Identity, kèm bộ bảo mật server-side đầy đủ (Authentication, Authorization theo Role/Policy, CSRF, XSS, SQL Injection prevention, Safe File Upload).

## Công nghệ sử dụng

- ASP.NET Core MVC (.NET 10)
- Entity Framework Core + SQLite
- ASP.NET Core Identity (Authentication/Authorization)
- Serilog (structured logging, ghi ra Console + File)
- Health Checks + ProblemDetails (RFC 7807)

## Cách chạy project

### 1. Clone repository

\`\`\`powershell
git clone https://github.com/Linem25/mini-stationery.git
cd mini-stationery/MiniStationery.Mvc
git checkout lab06-final-secure-mini-stationery
\`\`\`

### 2. Restore packages

\`\`\`powershell
dotnet restore
\`\`\`

### 3. Tạo database và chạy migration

\`\`\`powershell
dotnet ef database update
\`\`\`

### 4. Chạy ứng dụng

\`\`\`powershell
dotnet run
\`\`\`

## Tài khoản demo

| Role  | Email                      | Mật khẩu   | Quyền hạn |
|-------|------------------------------|------------|-----------|
| Admin | admin@ministationery.test    | Admin@123  | Toàn quyền |
| Staff | staff@ministationery.test    | Staff@123  | Chỉ xem |
| User  | user@ministationery.test     | User@123   | Không vào được trang quản trị |

## Route chính

| Route | Quyền |
|---|---|
| `GET /` | Public |
| `GET/POST /Account/Login` | Anonymous |
| `POST /Account/Logout` | Đã đăng nhập |
| `GET /Stationery` | Admin, Staff |
| `GET/POST /Stationery/Create` | Admin |
| `GET/POST /Stationery/Edit/{id}` | Admin |
| `POST /Stationery/Delete/{id}` | Admin |
| `GET /Stationery/Trash` | Admin |
| `POST /Stationery/Restore/{id}` | Admin |
| `GET /AuditLogs` | Admin |
| `GET /api/health/ready` | Public |
| `GET /api/stationery/{id}` | Public |

## Checklist bảo mật

- [x] CSRF token trên mọi form POST
- [x] Chống XSS (không dùng Html.Raw)
- [x] Chống SQL Injection (LINQ)
- [x] Upload an toàn (giới hạn extension + 2MB)
- [x] Soft Delete
- [x] Concurrency check (RowVersion)
- [x] Audit Log
- [x] Health Check
- [x] ProblemDetails có errorCode + traceId

## Ghi chú

Phần cấu hình Identity và debug lỗi migration có sự hỗ trợ từ AI trong quá trình thực hiện.