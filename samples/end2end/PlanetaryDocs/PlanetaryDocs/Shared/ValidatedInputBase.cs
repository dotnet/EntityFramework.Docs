using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using PlanetaryDocs.Domain;

namespace PlanetaryDocs.Shared
{
    /// <summary>
    /// Code for <see cref="ValidatedInput"/> component.
    /// </summary>
    public class ValidatedInputBase : ComponentBase
    {
        private bool focused = false;
        private string innerValue = string.Empty;
        private ValidationState validationState;

        /// <summary>
        /// Gets or sets the placeholder text.
        /// </summary>
        [Parameter]
        public string PlaceHolder { get; set; }

        /// <summary>
        /// Gets or sets the external value for data-binding.
        /// </summary>
        [Parameter]
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the callback used to notify on value changes.
        /// </summary>
        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the input
        /// should take focus on errors.
        /// </summary>
        [Parameter]
        public bool AutoFocus { get; set; }

        /// <summary>
        /// Gets or sets the method used to validate the input.
        /// </summary>
        [Parameter]
        public Func<string, ValidationState> Validate { get; set; }

        /// <summary>
        /// Gets or sets the state of the validation.
        /// </summary>
        [Parameter]
        public ValidationState Validation { get; set; }

        /// <summary>
        /// Gets or sets the callback to notify when the
        /// validation status changes.
        /// </summary>
        [Parameter]
        public EventCallback<ValidationState> ValidationChanged { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use
        /// a <c>textarea</c> instead of an <c>input</c>.
        /// </summary>
        [Parameter]
        public bool UseTextArea { get; set; }

        /// <summary>
        /// Gets or sets the tab index.
        /// </summary>
        [Parameter]
        public string TabIndex { get; set; } = "0";

        /// <summary>
        /// Gets or sets the reference to the <c>textarea</c> element.
        /// </summary>
        protected ElementReference TextAreaControl { get; set; }

        /// <summary>
        /// Gets or sets the reference to the <c>input</c> element.
        /// </summary>
        protected ElementReference InputControl { get; set; }

        /// <summary>
        /// Gets the active control based on the value of
        /// <see cref="UseTextArea"/>.
        /// </summary>
        protected ElementReference ActiveControl =>
            UseTextArea ? TextAreaControl : InputControl;

        /// <summary>
        /// Gets the class to apply for errors.
        /// </summary>
        protected string Error =>
            Validation != null && Validation.IsValid == false
            ? "error" : string.Empty;

        /// <summary>
        /// Gets or sets the internally tracked value.
        /// </summary>
        protected string InnerValue
        {
            get => innerValue;
            set
            {
                if (value != innerValue)
                {
                    innerValue = value;
                    Value = innerValue;
                    InvokeAsync(async () =>
                        await ValueChanged.InvokeAsync(Value));
                    OnValidate();
                }
            }
        }

        /// <summary>
        /// Called when validation is needed.
        /// </summary>
        public void OnValidate()
        {
            validationState = Validate(innerValue);

            if (Validation == null ||
                validationState.IsValid != Validation.IsValid ||
                validationState.Message != Validation.Message)
            {
                Validation = validationState;
                InvokeAsync(async () =>
                    await ValidationChanged.InvokeAsync(Validation));
            }

            if (!validationState.IsValid && AutoFocus)
            {
                InvokeAsync(
                    async () =>
                        await ActiveControl.FocusAsync());
            }
        }

        /// <summary>
        /// Method to focus the control.
        /// </summary>
        /// <returns>An asynchronous task.</returns>
        public async Task FocusAsync() => await ActiveControl.FocusAsync();

        /// <summary>
        /// Called when the value is initially set.
        /// </summary>
        protected override void OnParametersSet()
        {
            validationState = null;
            innerValue = Value;
            OnValidate();
            base.OnParametersSet();
        }

        /// <summary>
        /// Called after render.
        /// </summary>
        /// <param name="firstRender">A value indicating whether the call is in context of the first render.</param>
        /// <returns>An asynchronous task.</returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (AutoFocus && !focused)
            {
                focused = true;
                await ActiveControl.FocusAsync();
            }

            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
