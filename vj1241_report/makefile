all:
	@echo "Usage: make option"; \
	echo; \
	echo "Where option is one of:"; \
	echo "  pdf:   to make report.pdf from report.tex"; \
	echo "  clean: to clean the temporary files"

pdf:
	pdflatex report.tex && \
	bibtex report && \
	pdflatex report.tex && \
	pdflatex report.tex

clean:
	rm -f *.aux *.log *.maf *.mtc* *.out *.toc *.bbl *.blg *.synctex.gz *.fdb_latexmk *.fls
