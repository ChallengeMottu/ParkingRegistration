# Parking Registration 

O sistema Parking Registration é essencial para o funcionamento completo da solução Pulse, ele
coordena todo o processo de registro de Pátios (Parkings) e os recursos principais para a efetivação
do mapeamento e organização do espaço: Gateways e Zonas (Zones).
Trata-se de uma API Restful desenvolvida em .NET com foco em boas práticas e automação.

---

## 📌 Descrição do Projeto

O **Pulse** é um sistema voltado para a **gestão inteligente de pátios**, oferecendo funcionalidades como:

- **Cálculo de gateways necessários**: com base na cobertura de cada dispositivo e na área disponível do pátio.  
- **Gerenciamento de zonas**: cada pátio é dividido em 4 zonas padrão, e o sistema sugere medidas ideais durante o cadastro do pátio.  
- **Controle de pátios**: permite cadastro, atualização, listagem e remoção de pátios, garantindo organização da estrutura.

---

## 🏗 Arquitetura

A escolha pela arquitetura em **camadas** foi feita para garantir **organização, manutenibilidade e escalabilidade** do projeto.  

Cada camada possui uma responsabilidade bem definida, permitindo maior desacoplamento e facilitando a evolução da aplicação:

- **API**: concentra apenas a exposição de endpoints RESTful e o retorno das respostas no formato correto (com HATEOAS e status codes adequados), mantendo essa camada limpa e sem lógica de negócio.  
- **Application**: atua como orquestradora, chamando serviços e coordenando o fluxo entre domínio e infraestrutura. Isso facilita a implementação de regras de negócio sem acoplamento direto à camada de apresentação ou persistência.  
- **Domain**: é o coração do sistema, onde ficam as entidades e regras de negócio. Essa camada não depende de outras, o que garante independência e testabilidade das regras de negócio.  
- **Infrastructure**: cuida do acesso a dados e integrações externas. Dessa forma, mudanças no banco de dados ou em provedores externos impactam apenas esta camada, sem afetar diretamente o domínio ou a API.  

Essa abordagem segue princípios do **Domain-Driven Design (DDD)** e **Clean Architecture**, assegurando que a lógica de negócio permaneça isolada e independente de tecnologias ou frameworks específicos.  

---

## 🔧 Tecnologias Utilizadas

- **.NET 8**
- **C#**
- **Entity Framework Core**
- **HATEOAS**
- **Swagger / OpenAPI**
- **Banco de dados Oracle**
- **Paginação**

---

## 🚀 Como Rodar a API

1. Clonar o repositório
```bash
git clone https://github.com/seu-usuario/pulse-api.git
cd pulse-api
```

2. Restaurar dependências
```bash
dotnet restore
```

3. Configurar string de conexão

No **appsettings.json**, configure sua conexão com o banco de dados:
```bash
"ConnectionStrings": {
    "SystemPulse": "Server=localhost;Database=PulseDB;User Id=sa;Password=senha123;"
}
```

4. Aplicar migrations
```bash
dotnet ef database update
```

5. Rodar a API
```bash
dotnet run
```

6. Acessar documentação Swagger
```bash
http://localhost:5000/swagger
```

---

## ✅ Casos de Teste (API Endpoints)

Abaixo estão exemplos de requisições para testar os principais recursos da API (`Gateways`, `Parkings` e `Zones`).

### 🚗 Parking (Pátios)

#### ➕ Criar Parking (POST)
```http
POST /parkings
Content-Type: application/json

{
  "name": "Pátio Central",
  "location": {
    "street": "Rua A",
    "complement": "Próximo ao mercado",
    "neighborhood": "Centro",
    "cep": "12345-678",
    "city": "São Paulo",
    "state": "SP"
  },
  "availableArea": 5000,
  "capacity": 300
}
```

#### 📋 Listar Todos (GET)
```http
GET /parkings?pageNumber=1&pageSize=10
```

#### 🔍 Buscar por ID (GET)
```http
GET /parkings/{id_parking}
```

#### ✏️ Atualizar Parking (PUT)
```http
PUT /parkings/{id_parking}
Content-Type: application/json

{
  "name": "Pátio Central Atualizado",
  "location": {
    "street": "Rua B",
    "complement": "Ao lado da escola",
    "neighborhood": "Centro",
    "cep": "12345-678",
    "city": "São Paulo",
    "state": "SP"
  },
  "availableArea": 6000,
  "capacity": 350
}

```

#### ❌ Deletar Parking (DELETE)
```http
DELETE /parkings/{id_parking}
```
---

### 📡 Gateways

#### ➕ Criar Gateway (POST)
```http
POST /gateways
Content-Type: application/json

{
  "model": "Pulse-GTW-01",
  "status": 1,
  "macAddress": "00:1B:44:11:3A:B7",
  "lastIP": "192.168.0.15",
  "parkingId": {id_gateway}
}

```

#### 📋 Listar Todos (GET)
```http
GET /gateways?pageNumber=1&pageSize=10
```

### 🔍 Buscar por ID (GET)
```http
GET /gateways/{id_gateway}
```

#### 🔍 Buscar por MAC Address (GET)
```http
GET /gateways/mac/00:1B:44:11:3A:B7
```

#### ✏️ Atualizar Gateway (PUT)
```http
PUT /gateways/1
Content-Type: application/json

{
  "model": "Pulse-GTW-02",
  "status": 1,
  "macAddress": "00:1B:44:11:3A:B7",
  "lastIP": "192.168.0.20",
  "parkingId": 1
}
```

#### ❌ Deletar Gateway (DELETE)
```http
DELETE /gateways/{id_gateway}
```
---

### 🏷 Zones

#### ➕ Criar Zone (POST)
```http
POST /zones
Content-Type: application/json

{
  "name": "Zona A",
  "description": "Zona principal",
  "width": 25,
  "length": 50,
  "parkingId": 1
}
```

#### 📋 Listar Todas (GET)
```http
GET /zones?pageNumber=1&pageSize=10
```

#### 🔍 Buscar por ID (GET)
```http
GET /zones/{id_zone}
```

#### 📍 Buscar por Parking ID (GET)
```http
GET /zones/parking/{id_parking}
```

#### ✏️ Atualizar Zone (PUT)
```http
PUT /zones/{id_zone}
Content-Type: application/json

{
  "name": "Zona A Atualizada",
  "description": "Zona reformada",
  "width": 30,
  "length": 55,
  "parkingId": 1
}
```

#### ❌ Deletar Zone (DELETE)
```http
DELETE /zones/{id_zone}
```

---

## 📡 Status Codes da API

A aplicação segue os **padrões RESTful** e retorna os **status codes HTTP** adequados para cada operação.  
Isso facilita a integração com clientes externos e garante clareza nas respostas.

### 🔑 Status Codes Utilizados

- **200 OK** → Requisição bem-sucedida (usado em operações de consulta e atualização).  
- **201 Created** → Recurso criado com sucesso (usado em operações `POST`).  
- **204 No Content** → Recurso deletado com sucesso, sem corpo de resposta.  
- **400 Bad Request** → Erro de validação ou requisição malformada (ex: campos obrigatórios ausentes).  
- **404 Not Found** → Recurso não encontrado (ex: ID inexistente).  
- **422 Unprocessable Entity** → Quando a requisição foi entendida, mas contém erros de validação semântica (ex: medidas inválidas para zonas).  
- **500 Internal Server Error** → Erro inesperado no servidor.  

### 📝 Exemplo de Resposta com Erro

```json
{
  "status": 422,
  "error": "Unprocessable Entity",
  "message": "As medidas da zona não podem exceder a área disponível do pátio.",
  "path": "/zones"
}

---

## 👩‍💻 Grupo Desenvolvedor

- Gabriela de Sousa Reis RM558830
- Laura Amadeu Soares RM556690
- Raphael Lamaison Kim RM557914





