const URL_API = 'https://localhost:7081/api/v1/Documento';

async function enviarDocumento() {
    const codigoCliente = document.getElementById("codigoCliente").value;
    const inputArquivo = document.getElementById("arquivo");
    const arquivo = inputArquivo.files[0];

    if (!codigoCliente || !arquivo) {
        alert("Informe o codigo do cliente e selecine um arquivo");
        return;
    }

    const dadosArquivo = new FormData();
    dadosArquivo.append("arquivo", arquivo);

    const response = await fetch(`${URL_API}/upload/${codigoCliente}`, {
        method: "POST",
        body: dadosArquivo
    });

    if (response.ok) {
        alert("Documento enviado com sucesso!")
        document.getElementById("codigoCliente").value = "";
        document.getElementById("arquivo").value = "";
    } else {
        const erro = await response.json();
        alert("Erro: " + (erro.message || "Falha ao enviar o documento"));
    }
   
}
async function buscarDocumento() {

    const codigoCliente = document.getElementById("buscarCliente").value;

    if (!codigoCliente) {
        alert("Informe o codigo do cliente");
        return;
    }

    const response = await fetch(`${URL_API}/listar/${codigoCliente}`);
    const documentos = await response.json();
    const corpo = document.getElementById('corpoTabela');
    corpo.innerHTML = '';

    documentos.forEach(a => {
        // Maneira de separar o nome do arquivo com o tipo

        // Includes: corta caracteres especiais do texto | Split: separa o texto com o caracter '.' e o transforma em array | Pop: Retorna apenas o ultimo item do array
        const extensao = a.extensao || a.tipo || (nomeCompleto.includes('.') ? nomeCompleto.split('.').pop() : '');

        const nomeCompleto = a.nomeArquivo || a.nome || a.arquivo || `Documento ${a.codigoCliente}`;

        corpo.innerHTML += `
            <tr>
                <td>${a.codigoCliente}</td>
                <td>${nomeCompleto}</td>
                <td>${extensao}</td>
                <td>
                    <button class="btn-editar" onclick="baixarDocumento('${a.id}')">Baixar</button>
                    <button class="btn-excluir" onclick="excluirDocumento(${a.id}, '${nomeCompleto}')">Excluir</button>
                </td>
            </tr>`;
    });

}
async function excluirDocumento(id, nomeCompleto) {
    if (confirm(`Deseja realmente excluir o Documento ${nomeCompleto}?`)) {
        const response = await fetch(`${URL_API}/excluir/${id}`, { method: 'DELETE' });
        if (response.ok) {
            buscarDocumento();
        }
    }
}
function baixarDocumento(id) {
    location.href = `${URL_API}/download/${id}`;
}