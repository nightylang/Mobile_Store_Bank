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

/**
 * Mobile Store Bank - Asynchronous Order Purchase Pipeline Binder
 * Binds product card purchase actions to the backend transaction ledger
 */
window.InitializeProductActionListeners = function() {
    console.log("🛒 Product purchase event listener matrices mounted...");

    // Catch events at the document level to preserve functionality during dynamic re-renders
    document.body.addEventListener('click', async function(event) {
        
        // Target button click specifically by identifying our custom semantic trigger class
        const buyButton = event.target.closest('.msb-action-buy-trigger');
        if (!buyButton) return;

        event.preventDefault();

        // Traverse layout nodes upward to parse data cells from the parent container card
        const productCard = buyButton.closest('.msb-product-card');
        if (!productCard) return;

        // Parse runtime model properties injected inside data elements or text content
        const priceString = productCard.querySelector('.msb-item-price').textContent;
        const assetPoolLabel = productCard.querySelector('.msb-item-subtitle').textContent;
        
        // Extract raw floating-point numeric value out of the price tag text cell string
        const parsedAmount = parseFloat(priceString.replace(/[^0-9.-]+/g, ""));

        // Visual UX Feedback: Transition button state during network transit
        const primaryTextElement = buyButton.querySelector('span');
        const fallbackTextCache = primaryTextElement ? primaryTextElement.textContent : "Instantiate Order";
        if (primaryTextElement) primaryTextElement.textContent = "Processing Settle...";
        buyButton.disabled = true;
        buyButton.style.opacity = "0.5";

        try {
            // Dispatch AJAX cleartext fetch package to your dedicated REST API controller layer
            const response = await fetch('/api/ledger/settle', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'X-POS-Terminal-ID': 'WEB-INTERFACE-NODE',
                    'X-POS-Security-Token': 'POS-SECURE-KEY-HASH-V2'
                },
                body: JSON.stringify({
                    amount: parsedAmount,
                    targetAssetPool: "USD Core Ledger Pool" // Standard default settling lane
                })
            });

            if (!response.ok) {
                const errData = await response.json();
                throw new Error(errData.Error || `Server side execution error: ${response.status}`);
            }

            const result = await response.json();
            console.log("✅ Order commitment ledger verification success:", result);

            // Dynamically push state value adjustments over the global Proxy singleton instance
            // This instantly refreshes and flashes your glass navigation balance components!
            if (window.StoreBankState) {
                window.StoreBankState.usdPoolBalance = result.newInternalBalance;
            }

            // Notification Feedback Popup placeholder trigger
            alert(`Transaction Settled Successfully!\nReference: ${result.transactionRef}`);

        } catch (error) {
            console.error("❌ Order Processing Pipeline Aborted:", error.message);
            alert(`Settlement Anomaly: ${error.message}`);
        } finally {
            // Restore visual layout control states
            if (primaryTextElement) primaryTextElement.textContent = fallbackTextCache;
            buyButton.disabled = false;
            buyButton.style.opacity = "1";
        }
    });
};

