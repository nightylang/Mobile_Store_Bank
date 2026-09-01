/**
 * Mobile Store Bank - High-Utility Reactive Frontend State Hub
 * Built on native JS proxies to manage lightning-fast UI element data syncing
 */

// 1. Establish the global mutable data canvas object
const appDataState = {
    usdPoolBalance: 0.00,
    btcPoolBalance: 0.00000000,
    pendingClearanceUsd: 0.00,
    activeTicketsCount: 0,
    latestTransactionRef: "N/A"
};

// 2. The DOM UI Update Pipeline Binder Matrix
const uiRenderBindingEngine = {
    set(target, property, value) {
        // Only trigger DOM mutations if the tracking value actually drifted
        if (target[property] === value) return true;
        
        target[property] = value;

        // Reactive CSS Glow Class Injection Routine
        const applyFlashGlowEffect = (elementId) => {
            const el = document.getElementById(elementId);
            if (!el) return;
            el.classList.add('text-indigo-400', 'scale-[1.01]', 'duration-100');
            setTimeout(() => {
                el.classList.remove('text-indigo-400', 'scale-[1.01]');
            }, 800);
        };

        // UI Routing Mapping Conversions
        switch (property) {
            case 'usdPoolBalance':
                const usdEl = document.getElementById('usd-balance-display');
                if (usdEl) {
                    usdEl.textContent = `$${Number(value).toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`;
                    applyFlashGlowEffect('usd-balance-card');
                }
                break;
            case 'btcPoolBalance':
                const btcEl = document.getElementById('btc-balance-display');
                if (btcEl) btcEl.textContent = Number(value).toFixed(8);
                break;
            case 'pendingClearanceUsd':
                const pendingEl = document.getElementById('pending-clearance-display');
                if (pendingEl) pendingEl.textContent = `+$${Number(value).toFixed(2)}`;
                break;
            case 'activeTicketsCount':
                const ticketEl = document.getElementById('active-tickets-display');
                if (ticketEl) ticketEl.textContent = `${value} Tickets`;
                break;
        }
        return true;
    }
};

// 3. Instantiate the global thread-safe state proxy singleton instance
window.StoreBankState = new Proxy(appDataState, uiRenderBindingEngine);

/**
 * High-Density Polling Synchronization Loop
 * Intercepts payload matrices over cleartext HTTP network pipes
 */
window.InitializeLedgerPollingLoop = function(pollingIntervalMs = 4000) {
    console.log("🚀 Real-Time HTTP Ledger Sync Pipeline Active...");

    async function executePollCycle() {
        try {
            // Fetch structural data directly from your REST API endpoint context
            const response = await fetch('/api/ledger/state-summary', {
                method: 'GET',
                headers: { 'Accept': 'application/json' }
            });

            if (!response.ok) throw new Error(`HTTP network error proxy state code: ${response.status}`);
            
            const data = await response.json();

            // Direct assignment triggers the Proxy Handler's custom 'set()' mutations
            window.StoreBankState.usdPoolBalance = data.usdBalance;
            window.StoreBankState.btcPoolBalance = data.btcBalance;
            window.StoreBankState.pendingClearanceUsd = data.pendingUsd;
            window.StoreBankState.activeTicketsCount = data.openTicketsCount;

        } catch (error) {
            console.warn("⚠️ Telemetry Handshake Dropped (Verify local cleartext channel binding):", error.message);
        }
    }

    // Initialize immediate execution on startup, then scale intervals safely
    executePollCycle();
    setInterval(executePollCycle, pollingIntervalMs);
};
            
