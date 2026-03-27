async function downloadCv() {
    try {
        const response = await fetch('Leonardo_Ganas_CV.pdf');
        if (!response.ok) throw new Error('Network response was not ok');
        
        const blob = await response.blob();
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        
        link.href = url;
        link.download = 'Leonardo-Ganas-CV.pdf';
        
        // Append to body is required for some browsers
        document.body.appendChild(link);
        link.click();
        
        // Cleanup
        document.body.removeChild(link);
        window.URL.revokeObjectURL(url);
    } catch (error) {
        console.error('Download failed:', error);
        // Fallback to simple link if fetch fails
        window.open('Leonardo_Ganas_CV.pdf', '_blank');
    }
}