FROM ollama/ollama

# Este comando baixa o modelo durante a construção da imagem.
# É mais confiável do que tentar baixar depois que o serviço sobe.
RUN ollama pull phi4-mini:q4_0

# A imagem base do ollama/ollama já sabe que o comando para iniciar é "ollama serve".
# Não precisamos de mais nada aqui.