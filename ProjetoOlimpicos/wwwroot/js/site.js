/* ============================================================================
   OLYMPIC EDITORIAL REBIRTH - INTERACTIVE SCRIPTS
   ============================================================================ */

document.addEventListener('DOMContentLoaded', function() {
    // 1. DROPDOWN NAVEGAÇÃO
    const dropdownButtons = document.querySelectorAll('.btn-dropdown');
    
    dropdownButtons.forEach(btn => {
        btn.addEventListener('click', function(e) {
            e.stopPropagation();
            const menu = this.nextElementSibling;
            const isOpen = menu.style.display === 'block';
            
            // Fechar outros dropdowns
            document.querySelectorAll('.dropdown-menu').forEach(m => m.style.display = 'none');
            document.querySelectorAll('.btn-dropdown').forEach(b => b.setAttribute('aria-expanded', 'false'));
            
            // Abrir/Fechar atual
            menu.style.display = isOpen ? 'none' : 'block';
            this.setAttribute('aria-expanded', !isOpen);
        });
    });
    
    // Fechar dropdown ao clicar fora
    document.addEventListener('click', function(e) {
        if (!e.target.closest('.nav-dropdown')) {
            document.querySelectorAll('.dropdown-menu').forEach(menu => {
                menu.style.display = 'none';
            });
            document.querySelectorAll('.btn-dropdown').forEach(btn => {
                btn.setAttribute('aria-expanded', 'false');
            });
        }
    });

    // 2. ANIMAÇÕES DE ENTRADA (FADE-IN)
    const observerOptions = {
        threshold: 0.1
    };

    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.style.opacity = '1';
                entry.target.style.transform = 'translateY(0)';
            }
        });
    }, observerOptions);

    document.querySelectorAll('.editorial-card, .editorial-list-item, .quick-fact').forEach(el => {
        el.style.opacity = '0';
        el.style.transform = 'translateY(20px)';
        el.style.transition = 'opacity 0.6s ease-out, transform 0.6s ease-out';
        observer.observe(el);
    });
});
