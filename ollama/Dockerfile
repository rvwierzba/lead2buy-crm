FROM ollama/ollama

# O RUN baixa o modelo durante a construção da imagem, o que é mais confiável.
RUN ollama pull phi4-mini:q4_0

# O ENTRYPOINT da imagem base já é "ollama", então passamos "serve" como comando padrão.
CMD [ "serve" ]