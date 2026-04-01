/* =========================================
   LUMIÈRE — JavaScript
   ========================================= */

// ── CURSOR PERSONALIZADO ──
const cursor = document.getElementById('cursor');
const follower = document.getElementById('cursorFollower');
let mouseX = 0, mouseY = 0, followerX = 0, followerY = 0;

document.addEventListener('mousemove', (e) => {
  mouseX = e.clientX;
  mouseY = e.clientY;
  cursor.style.left = mouseX + 'px';
  cursor.style.top = mouseY + 'px';
});

function animateFollower() {
  followerX += (mouseX - followerX) * 0.12;
  followerY += (mouseY - followerY) * 0.12;
  follower.style.left = followerX + 'px';
  follower.style.top = followerY + 'px';
  requestAnimationFrame(animateFollower);
}
animateFollower();

document.querySelectorAll('a, button').forEach(el => {
  el.addEventListener('mouseenter', () => {
    cursor.style.transform = 'translate(-50%, -50%) scale(2)';
    follower.style.transform = 'translate(-50%, -50%) scale(0.5)';
  });
  el.addEventListener('mouseleave', () => {
    cursor.style.transform = 'translate(-50%, -50%) scale(1)';
    follower.style.transform = 'translate(-50%, -50%) scale(1)';
  });
});

// ── NAVBAR SCROLL ──
const navbar = document.getElementById('navbar');
window.addEventListener('scroll', () => {
  navbar.classList.toggle('scrolled', window.scrollY > 50);
});

// ── MOBILE MENU ──
const menuToggle = document.getElementById('menuToggle');
const mobileMenu = document.getElementById('mobileMenu');

menuToggle.addEventListener('click', () => {
  mobileMenu.classList.toggle('open');
});

document.querySelectorAll('.mobile-link').forEach(link => {
  link.addEventListener('click', () => mobileMenu.classList.remove('open'));
});

// ── DADOS DOS PRODUTOS ──
const produtos = [
  {
    id: 1, nome: 'Sérum Vitamina C Radiante', categoria: 'skincare',
    preco: 189.90, precoOriginal: 249.90,
    desc: 'Concentrado 20% de vitamina C pura. Ilumina e unifica o tom da pele.',
    emoji: '✨', badge: 'Mais Vendido'
  },
  {
    id: 2, nome: 'Hidratante Glow Facial', categoria: 'skincare',
    preco: 149.90, precoOriginal: null,
    desc: 'Textura gel-creme leve com ácido hialurônico. Hidratação 72 horas.',
    emoji: '💧', badge: 'Novo'
  },
  {
    id: 3, nome: 'Batom Matte Velvet', categoria: 'maquiagem',
    preco: 79.90, precoOriginal: 99.90,
    desc: 'Formula ultrapigmentada com acabamento aveludado. 8h de duração.',
    emoji: '💄', badge: null
  },
  {
    id: 4, nome: 'Base HD Cobertura Total', categoria: 'maquiagem',
    preco: 129.90, precoOriginal: null,
    desc: 'Cobertura buildável com finish natural. FPS 30 integrado.',
    emoji: '🌟', badge: 'Novo'
  },
  {
    id: 5, nome: 'Eau de Parfum Floral', categoria: 'perfumaria',
    preco: 299.90, precoOriginal: 389.90,
    desc: 'Notas de Rosa Búlgara, Jasmim e Sândalo. Fragrância feminina e sofisticada.',
    emoji: '🌹', badge: 'Mais Vendido'
  },
  {
    id: 6, nome: 'Perfume Noite Dourada', categoria: 'perfumaria',
    preco: 259.90, precoOriginal: null,
    desc: 'Amadeirado oriental com notas de Baunilha, Âmbar e Patchouli.',
    emoji: '✦', badge: null
  },
  {
    id: 7, nome: 'Máscara Restauradora', categoria: 'cabelos',
    preco: 119.90, precoOriginal: 159.90,
    desc: 'Tratamento intensivo com Queratina e Manteiga de Karité. Brilho máximo.',
    emoji: '💆', badge: 'Mais Vendido'
  },
  {
    id: 8, nome: 'Óleo Capilar Argan', categoria: 'cabelos',
    preco: 89.90, precoOriginal: null,
    desc: 'Óleo de Argan Marroquino puro. Sela as cutículas e elimina o frizz.',
    emoji: '🌿', badge: 'Novo'
  },
  {
    id: 9, nome: 'Esfoliante Facial Enzimático', categoria: 'skincare',
    preco: 109.90, precoOriginal: 139.90,
    desc: 'Esfoliação suave com enzimas de mamão papaia. Pele sedosa em minutos.',
    emoji: '🌸', badge: null
  },
  {
    id: 10, nome: 'Paleta de Sombras Dusk', categoria: 'maquiagem',
    preco: 179.90, precoOriginal: 219.90,
    desc: '12 tons quentes. Acabamentos matte, shimmer e glitter para qualquer look.',
    emoji: '🎨', badge: 'Mais Vendido'
  },
  {
    id: 11, nome: 'Creme Antienvelhecimento', categoria: 'skincare',
    preco: 219.90, precoOriginal: null,
    desc: 'Retinol 0.3% + Peptídeos. Reduz linhas de expressão visivelmente em 28 dias.',
    emoji: '⚡', badge: 'Novo'
  },
  {
    id: 12, nome: 'Shampoo Low-Poo Hidra', categoria: 'cabelos',
    preco: 74.90, precoOriginal: 94.90,
    desc: 'Limpeza suave sem sulfatos. Ideal para cabelos quimicamente tratados.',
    emoji: '🫧', badge: null
  },
];

// ── RENDERIZAR PRODUTOS ──
let carrinho = [];
let filtroAtivo = 'todos';

function formatarPreco(valor) {
  return valor.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
}

function renderizarProdutos(filtro = 'todos') {
  const grid = document.getElementById('produtosGrid');
  const filtrados = filtro === 'todos' ? produtos : produtos.filter(p => p.categoria === filtro);

  grid.innerHTML = filtrados.map((p, i) => `
    <article class="produto-card reveal" style="animation-delay: ${i * 0.07}s" data-id="${p.id}">
      <div class="card-img-wrap">
        <div class="card-img-placeholder">${p.emoji}</div>
        ${p.badge ? `<span class="card-badge ${p.badge === 'Novo' ? 'novo' : ''}">${p.badge}</span>` : ''}
      </div>
      <div class="card-body">
        <span class="card-cat">${p.categoria}</span>
        <h3 class="card-nome">${p.nome}</h3>
        <p class="card-desc">${p.desc}</p>
        <div class="card-footer">
          <div class="card-preco">
            ${p.precoOriginal ? `<small>${formatarPreco(p.precoOriginal)}</small>` : ''}
            ${formatarPreco(p.preco)}
          </div>
          <button class="card-add" onclick="adicionarAoCarrinho(${p.id})" title="Adicionar ao carrinho">
            +
          </button>
        </div>
      </div>
    </article>
  `).join('');

  // Ativar reveals
  setTimeout(ativarReveals, 100);
}

// ── FILTROS ──
document.querySelectorAll('.filtro-btn').forEach(btn => {
  btn.addEventListener('click', () => {
    document.querySelectorAll('.filtro-btn').forEach(b => b.classList.remove('active'));
    btn.classList.add('active');
    filtroAtivo = btn.dataset.filter;
    renderizarProdutos(filtroAtivo);
  });
});

// Filtro por categoria (cards visuais)
document.querySelectorAll('.cat-card').forEach(card => {
  card.addEventListener('click', () => {
    const filtro = card.dataset.filter;
    document.querySelectorAll('.filtro-btn').forEach(b => {
      b.classList.toggle('active', b.dataset.filter === filtro);
    });
    filtroAtivo = filtro;
    renderizarProdutos(filtro);
    document.getElementById('produtos').scrollIntoView({ behavior: 'smooth' });
  });
});

// ── CARRINHO ──
function adicionarAoCarrinho(id) {
  const produto = produtos.find(p => p.id === id);
  if (!produto) return;

  const existente = carrinho.find(item => item.id === id);
  if (existente) {
    existente.qtd += 1;
  } else {
    carrinho.push({ ...produto, qtd: 1 });
  }

  atualizarCarrinho();
  mostrarToast(`${produto.nome} adicionado ao carrinho ✓`);
}

function removerDoCarrinho(id) {
  carrinho = carrinho.filter(item => item.id !== id);
  atualizarCarrinho();
}

function atualizarCarrinho() {
  const count = carrinho.reduce((acc, item) => acc + item.qtd, 0);
  const total = carrinho.reduce((acc, item) => acc + (item.preco * item.qtd), 0);

  document.getElementById('cartCount').textContent = count;

  const itemsContainer = document.getElementById('cartItems');
  const footer = document.getElementById('cartFooter');

  if (carrinho.length === 0) {
    itemsContainer.innerHTML = `
      <div class="cart-empty">
        <p>Seu carrinho está vazio.</p>
        <small>Adicione produtos para começar.</small>
      </div>`;
    footer.style.display = 'none';
  } else {
    itemsContainer.innerHTML = carrinho.map(item => `
      <div class="cart-item">
        <div class="cart-item-img">${item.emoji}</div>
        <div class="cart-item-info">
          <strong>${item.nome}</strong>
          <span>Qtd: ${item.qtd}</span>
        </div>
        <div class="cart-item-price">${formatarPreco(item.preco * item.qtd)}</div>
        <button class="cart-item-remove" onclick="removerDoCarrinho(${item.id})">✕</button>
      </div>
    `).join('');
    footer.style.display = 'block';
    document.getElementById('cartTotal').textContent = formatarPreco(total);
  }
}

// Abrir/fechar carrinho
const cartBtn = document.getElementById('cartBtn');
const cartSidebar = document.getElementById('cartSidebar');
const cartOverlay = document.getElementById('cartOverlay');
const cartClose = document.getElementById('cartClose');

function abrirCarrinho() {
  cartSidebar.classList.add('open');
  cartOverlay.classList.add('open');
  document.body.style.overflow = 'hidden';
}

function fecharCarrinho() {
  cartSidebar.classList.remove('open');
  cartOverlay.classList.remove('open');
  document.body.style.overflow = '';
}

cartBtn.addEventListener('click', abrirCarrinho);
cartClose.addEventListener('click', fecharCarrinho);
cartOverlay.addEventListener('click', fecharCarrinho);

// ── TOAST ──
let toastTimer;
function mostrarToast(mensagem) {
  const toast = document.getElementById('toast');
  toast.textContent = mensagem;
  toast.classList.add('show');
  clearTimeout(toastTimer);
  toastTimer = setTimeout(() => toast.classList.remove('show'), 2800);
}

// ── NEWSLETTER ──
document.getElementById('nlForm').addEventListener('submit', (e) => {
  e.preventDefault();
  document.getElementById('nlSuccess').classList.add('show');
  e.target.reset();
  setTimeout(() => document.getElementById('nlSuccess').classList.remove('show'), 5000);
});

// ── FORMULÁRIO DE CONTATO ──
document.getElementById('contatoForm').addEventListener('submit', (e) => {
  e.preventDefault();
  document.getElementById('formSuccess').classList.add('show');
  e.target.reset();
  setTimeout(() => document.getElementById('formSuccess').classList.remove('show'), 5000);
});

// ── SCROLL REVEAL ──
function ativarReveals() {
  const observer = new IntersectionObserver((entries) => {
    entries.forEach((entry, i) => {
      if (entry.isIntersecting) {
        setTimeout(() => entry.target.classList.add('visible'), i * 80);
        observer.unobserve(entry.target);
      }
    });
  }, { threshold: 0.1 });

  document.querySelectorAll('.reveal').forEach(el => observer.observe(el));
}

// Adicionar classe reveal a seções
function configurarReveals() {
  const seletores = [
    '.cat-card', '.dep-card', '.valor',
    '.stat', '.sobre-text h2', '.section-header',
    '.info-item', '.editorial-text h2', '.editorial-text p'
  ];
  seletores.forEach(sel => {
    document.querySelectorAll(sel).forEach(el => el.classList.add('reveal'));
  });
  ativarReveals();
}

// ── SMOOTH SCROLL para links internos ──
document.querySelectorAll('a[href^="#"]').forEach(anchor => {
  anchor.addEventListener('click', (e) => {
    e.preventDefault();
    const target = document.querySelector(anchor.getAttribute('href'));
    if (target) target.scrollIntoView({ behavior: 'smooth' });
  });
});

// ── INICIALIZAR ──
document.addEventListener('DOMContentLoaded', () => {
  renderizarProdutos();
  configurarReveals();
});
