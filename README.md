# 🎓 Gerador de Certificados Online

* O **Gerador de Certificados Online** é uma aplicação responsável por automatizar a emissão de certificados para cursos.

* O sistema permite cadastrar cursos, solicitar a geração de certificados para alunos e acompanhar o processamento até a disponibilização dos documentos em formato PDF compactados em um arquivo ZIP.

* O processamento dos certificados ocorre de forma assíncrona, garantindo melhor desempenho e escalabilidade da aplicação.

<!-- <p align="center">
./.docs/home.gif
</p> -->

## Funcionalidades

### 🔐 1. Módulo de Usuários e Autenticação

<!-- <p align="center">
<img src="./.docs/auth.gif">
</p> -->

### Requisitos Funcionais

* O sistema deve permitir o cadastro de usuários por email e senha
* O sistema deve permitir a autenticação de usuários por email e senha
* O sistema deve emitir um token JWT após autenticação válida
* O sistema deve validar o token JWT nas rotas protegidas

### Regras de Negócio

> ** O email deve ser válido e único  
> ** A senha deve possuir no mínimo 8 caracteres  
> ** A senha deve conter ao menos um dígito e um caractere não alfanumérico  
> ** Credenciais e a chave de assinatura JWT não devem ser armazenadas no código-fonte  
> ** As rotas de cadastro e login são públicas  
> ** As demais rotas exigem `Authorization: Bearer {token}`

#### Endpoints

| Método | Rota | Descrição | Sucesso |
|---------|---------|---------|---------|
| `POST` | `/auth/cadastro` | Cadastra usuário | `201 Created` |
| `POST` | `/auth/login` | Autentica usuário e emite JWT | `200 OK` |

---

### 📚 2. Módulo de Cursos

<!-- <p align="center">
<img src="./.docs/cursos.gif">
</p> -->

### Requisitos Funcionais

* O sistema deve permitir cadastrar cursos
* O sistema deve permitir consultar cursos pelo identificador
* O sistema deve permitir associar solicitações de geração de certificados a um curso existente

### Regras de Negócio

* Campos obrigatórios:

  * Nome
  * Carga horária
  * Data de conclusão

> ** O curso deve possuir nome com no máximo 200 caracteres  
> ** A descrição do curso é opcional e deve possuir no máximo 500 caracteres  
> ** A carga horária deve ser maior que zero  
> ** A data de conclusão é obrigatória  
> ** Um curso inexistente não pode receber solicitações de geração de certificados

#### Endpoints

| Método | Rota | Descrição | Sucesso |
|---------|---------|---------|---------|
| `POST` | `/cursos` | Cadastra curso | `201 Created` |
| `GET` | `/cursos/{cursoId}` | Consulta curso | `200 OK` |

---

### 📄 3. Módulo de Certificados

<!-- <p align="center">
<img src="./.docs/certificados.gif">
</p> -->

### Requisitos Funcionais

* O sistema deve permitir solicitar a geração de certificados para um ou mais alunos
* O sistema deve persistir um certificado para cada aluno informado
* O sistema deve gerar um PDF individual para cada aluno
* O sistema deve registrar caminho, data de geração e status do certificado
* O sistema deve permitir consultar o status do processamento
* O sistema deve permitir listar certificados gerados
* O sistema deve permitir realizar o download de um arquivo ZIP contendo os certificados

### Regras de Negócio

* Campos obrigatórios:

  * Curso
  * Nome do aluno

> ** A solicitação de geração deve possuir pelo menos um aluno  
> ** O nome de cada aluno é obrigatório e deve possuir no máximo 200 caracteres  
> ** Um curso com geração de certificados em andamento não pode receber nova solicitação  
> ** Nestes casos o sistema deve retornar `409 Conflict`  
> ** O sistema deve gerar um PDF individual para cada aluno solicitado  
> ** Após a geração dos PDFs o sistema deve produzir um arquivo ZIP para download

#### Endpoints

| Método | Rota | Descrição | Sucesso |
|---------|---------|---------|---------|
| `POST` | `/cursos/{cursoId}/certificados` | Solicita geração | `202 Accepted` |
| `GET` | `/cursos/{cursoId}/status` | Consulta processamento | `200 OK` |
| `GET` | `/cursos/{cursoId}/certificados` | Lista certificados | `200 OK` |
| `GET` | `/cursos/{cursoId}/certificados/download` | Baixa o ZIP | `200 OK` (`application/zip`) |

---

## 🔄 Fluxo de Processamento

O sistema utiliza processamento assíncrono para geração dos certificados.

Os status possíveis durante o processamento são:

* Pendente
* Gerando Certificados
* Gerando ZIP
* Concluído
* Falha

Uma solicitação permanece associada ao curso até a conclusão do processamento, impedindo novas gerações simultâneas para o mesmo curso.

---

## 🔍 Testes

O projeto possui testes automatizados para garantir o funcionamento
das principais regras de negócio e dos fluxos da API.

### Testes de Unidade

Testam as regras de negócio de forma isolada, incluindo:

- Usuários e Autenticação
- Cursos
- Geração de Certificados

### Testes de Integração

Testam a integração entre as principais camadas da aplicação,
incluindo os endpoints da API, autenticação, persistência e
processamento das funcionalidades.

Os testes de integração abrangem:

- Usuários e Autenticação
- Cursos
- Geração de Certificados

---

## 📋 Resumo dos Módulos

| Módulo | Principais funcionalidades |
|---------|---------|
| 🔐 Usuários e Autenticação | Cadastro, login, emissão e validação de JWT |
| 📚 Cursos | Cadastro e consulta de cursos |
| 📄 Certificados | Geração, processamento, consulta e download de certificados |

---

## Como utilizar

1. Clone o repositório ou baixe o código-fonte.
2. Abra o terminal na pasta raiz do projeto.
3. Restaure as dependências:

   ```bash
   dotnet restore
   ```

4. Execute a aplicação:

   ```bash
   dotnet run
   ```

## Requisitos

- .NET 10 SDK
- SQL Server
- RabbitMQ
<!-- - CloudAMQP -->

## 👩‍💻 Colaboradores

1. Júlia Hartmann - [@juliaahartmann](https://github.com/JuliaaHartmann)
2. Natália Bortoli Vieira - [@nataliavieirab](https://github.com/nataliavieirab)
3. Revisado pela [Academia do Programador](https://academiadoprogramador.com.br)