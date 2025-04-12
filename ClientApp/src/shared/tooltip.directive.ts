import {
  Directive,
  ElementRef,
  Input,
  HostListener,
  Renderer2,
} from '@angular/core';

@Directive({
  selector: '[tooltip]',
})
export class TooltipDirective {
  @Input('tooltip') tooltipText = '';
  tooltipElement?: HTMLElement;

  constructor(private el: ElementRef, private renderer: Renderer2) {}

  @HostListener('mouseenter')
  onMouseEnter() {
    if (!this.tooltipElement) {
      this.tooltipElement = this.renderer.createElement('div');

      if (this.tooltipElement) {
        this.tooltipElement.className = 'custom-tooltip';
        this.tooltipElement.textContent = this.tooltipText;
      }

      this.renderer.appendChild(document.body, this.tooltipElement);

      const rect = this.el.nativeElement.getBoundingClientRect();
      const scrollY = window.scrollY || window.pageYOffset;

      this.renderer.setStyle(
        this.tooltipElement,
        'top',
        `${rect.top + scrollY - 40}px`
      );
      this.renderer.setStyle(this.tooltipElement, 'left', `${rect.left}px`);
    }
  }

  @HostListener('mouseleave')
  onMouseLeave() {
    if (this.tooltipElement) {
      this.renderer.removeChild(document.body, this.tooltipElement);
      this.tooltipElement = undefined;
    }
  }
}
