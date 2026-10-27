# CP5 — Biblioteca REST API (.NET 8)

Projeto da disciplina **Advanced Business Development with .NET (CP5)**, baseado nos exemplos das aulas, principalmente Controllers, UseCases, Repository, Oracle/Entity Framework Core, Swagger e autenticação JWT. Não utiliza requisitos do Challenge 2026.

## Abrir no Visual Studio

1. Instale o SDK **.NET 8**.
2. Abra `CP5.Biblioteca.sln` (dois cliques).
3. Defina `CP5.API` como **Projeto de Inicialização**, se necessário.
4. No Oracle, execute `database/oracle.sql` e `database/02_usuario.sql` **apenas se ainda não tiver criado as tabelas**.
5. Copie `CP5.API/appsettings.Development.json.example` para `CP5.API/appsettings.Development.json`.
6. No arquivo copiado, preencha a conexão Oracle do seu usuário e a `Secretkey` JWT (mínimo 32 bytes). Mantenha o arquivo local fora do Git.
7. Pressione **Ctrl+F5**. O Swagger abre em `http://localhost:5080/swagger`.

Exemplo de arquivo de configuração (substitua os valores):

```json
{
  "Logging": { "LogLevel": { "Default": "Information", "Microsoft.AspNetCore": "Warning" } },
  "ConnectionStrings": {
    "Oracle": "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=oracle.fiap.com.br)(PORT=1521))(CONNECT_DATA=(SERVER=DEDICATED)(SID=ORCL)));User Id=SEU_USUARIO;Password=SUA_SENHA;"
  },
  "Secretkey": "SUBSTITUA_POR_UMA_CHAVE_JWT_DE_32_CARACTERES_OU_MAIS"
}
```

**Importante:** `appsettings.Development.json` está no `.gitignore`, pois contém credenciais. A chave JWT é necessária para o login e para testar endpoints protegidos, mas **não** é necessária para cadastrar usuários.

## Testar pelo Swagger — fluxo simples

### 1. Criar administrador (sem token)

**POST `/api/auth/registrar-admin`**

```json
{
  "nome": "Joao",
  "email": "joao@teste.com",
  "password": "Joao123!"
}
```

Resposta esperada: **200 OK**, `role: "admin"`. Não exige JWT nem chave de instalação.

### 2. Criar leitor (sem token)

**POST `/api/auth/registrar`**

```json
{
  "nome": "Ana",
  "email": "ana@teste.com",
  "password": "Ana123!"
}
```

Resposta esperada: **200 OK**, `role: "leitor"`.

### 3. Fazer login com Joao

**POST `/api/auth`**

```json
{
  "email": "joao@teste.com",
  "senha": "Joao123!"
}
```

Copie o campo `jwt` da resposta, clique em **Authorize** no Swagger e cole o token (sem prefixar `Bearer`).

### 4. Cadastrar livro

**POST `/api/livros`** com o JWT de Joao (admin):

```json
{
  "id": 0,
  "titulo": "Dom Casmurro",
  "autor": "Machado de Assis",
  "anoPublicacao": 1899
}
```

Depois confira o registro no Oracle:

```sql
SELECT ID, NOME, EMAIL, ROLE FROM CP5_USUARIO;
SELECT * FROM CP5_LIVRO;
```

**Teste de autorização:** ao fazer login com Ana (leitor) e tentar `POST /api/livros`, a API deve retornar **403 Forbidden**. Sem JWT, os endpoints protegidos devem retornar **401 Unauthorized**. A criação pública de admin é uma simplificação **somente para o exercício acadêmico**, não uma prática adequada para produção.

## Endpoints

- `POST /api/auth/registrar` — cria leitor, público.
- `POST /api/auth/registrar-admin` — cria administrador, público.
- `POST /api/auth` — login com JWT.
- `GET/POST/PUT/DELETE /api/livros` — CRUD de livros; leitura/alteração conforme perfil.
- `GET/POST/PUT/DELETE /api/emprestimos` — CRUD de empréstimos; leitura/alteração conforme perfil.
- `GET /api/endereco/{cep}` — serviço externo ViaCEP.

As consultas usam paginação e links HATEOAS.

## Arquitetura e conceitos

- `CP5.Domain`: entidades, interfaces e `OperationResult`.
- `CP5.Application`: DTOs e UseCases.
- `CP5.Infrastructure`: EF Core/Oracle, Repository e ViaCEP via RestSharp.
- `CP5.API`: Controllers, JWT, Swagger e autorização por perfil.
- `CP5.Tests`: testes xUnit/Moq com padrão Arrange, Act, Assert.

A aplicação usa `OracleSQLCompatibility.DatabaseVersion19` para compatibilidade com o Oracle da FIAP e atributos `[Table]`/`[Column]` que mapeiam o modelo C# para os nomes das colunas Oracle.

## Compilar e executar testes

```powershell
dotnet build CP5.API/CP5.API.csproj
dotnet test CP5.Tests/CP5.Tests.csproj
```

A compilação e execução dos testes precisam ser confirmadas no Visual Studio com .NET 8; os testes funcionais/de integração ainda precisam ser complementados para atender integralmente ao CP5.

## Entrega

- Repositório público: [INSERIR LINK]
- Vídeo de demonstração (máximo de 5 minutos): [INSERIR LINK]
- Integrantes e RMs: [PREENCHER]
