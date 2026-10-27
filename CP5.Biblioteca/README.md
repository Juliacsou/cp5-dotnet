# CP5 — Biblioteca REST API (.NET 8)

Projeto exclusivamente do CP5, fundamentado no material de aula do professor, especialmente a Aula 08 (UserEntity, UserRepository, UserUseCase, JWT e HATEOAS). Não utiliza requisitos do Challenge 2026.

## Preparação no Visual Studio

1. Instale o **SDK .NET 8**. Abra `CP5.Biblioteca.sln` com dois cliques no Visual Studio. Configure `CP5.API` como projeto de inicialização (botão direito > Definir como Projeto de Inicialização) e utilize o perfil `CP5.API` para iniciar com F5 ou Ctrl+F5. O navegador abrirá `/swagger` automaticamente.
2. Você já executou `database/oracle.sql`? **Não execute novamente**. Execute apenas `database/02_usuario.sql` no mesmo schema Oracle.
3. Em `CP5.API`, **copie** `appsettings.Development.json.example` para `appsettings.Development.json`. Edite o arquivo com a mesma conexão Oracle do SQL Developer e uma `Secretkey` longa (mínimo 32 caracteres). O `.gitignore` impede o versionamento do arquivo com a senha.
4. Execute:

```powershell
dotnet restore CP5.API/CP5.API.csproj
dotnet build CP5.API/CP5.API.csproj
dotnet run --project CP5.API
```

5. O perfil `CP5.API/Properties/launchSettings.json` define o ambiente `Development`, a porta `5080` e a abertura automática de `/swagger` pelo Visual Studio. O comando `dotnet run --project CP5.API` também usa o perfil de inicialização por padrão. Em execução pelo terminal, o endereço será `http://localhost:5080/swagger`, mas o navegador pode não abrir automaticamente. Para usar outra porta ou comportamento, altere o `launchSettings.json`.

## Login e permissões

1. Swagger → `POST /api/auth/registrar`: `{"nome":"Aluno","email":"aluno@exemplo.com","password":"Senha123"}`.
2. O cadastro sempre cria `Role=leitor`, sem autorização para alterar livros ou empréstimos.
3. No Oracle, para usar a demonstração como admin, execute `UPDATE CP5_USUARIO SET ROLE='admin' WHERE EMAIL='aluno@exemplo.com'; COMMIT;`.
4. Swagger → `POST /api/auth`: `{"email":"aluno@exemplo.com","senha":"Senha123"}`.
5. Copie `Jwt` da resposta, clique em **Authorize** no Swagger e cole o token (sem prefixo `Bearer`).
6. Teste `GET /api/livros`, `POST /api/livros`, empréstimos e `GET /api/endereco/{cep}`.
7. Remova o token no Authorize e verifique resposta 401 em rota protegida.

**Nota didática de segurança**: para ficar alinhado ao código da Aula 08, a senha é comparada diretamente no repositório, conforme demonstrado pelo professor. Isso é inadequado para produção; não reutilize senhas reais. A funcionalidade de registro não permite escolher papel de administrador. A chave JWT e a senha Oracle não devem ir ao GitHub.

## Organização

- `CP5.Domain`: entidades e interfaces, exceções, OperationResult.
- `CP5.Application`: use cases, DTOs, paginação e JWT.
- `CP5.Infrastructure`: EF Core, Oracle, repositories e ViaCEP via RestSharp.
- `CP5.API`: controllers, Swagger, autenticação e autorização.
- `CP5.Tests`: testes unitários xUnit/Moq (testes funcionais com Oracle ainda pendentes).

## Testes

```powershell
dotnet test CP5.Tests/CP5.Tests.csproj
```

**Validação pendente**: este pacote não foi compilado neste ambiente, pois não há SDK .NET instalado aqui. Antes de entregar é necessário compilar e testar localmente, corrigir erros caso ocorram e complementar os testes de integração exigidos pelo CP5. Não há banco SQLite nem novas tecnologias arquiteturais introduzidas.

## Entrega
Vídeo demonstrativo (máximo 5 minutos): [INSERIR LINK]  
Integrantes / RM: [PREENCHER]
