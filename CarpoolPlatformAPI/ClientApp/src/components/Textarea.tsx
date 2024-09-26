import React, { ChangeEvent } from "react";

interface TextareaProps {
  label: string;
  id: string;
  name: string;
  value: string;
  onChange: (e: ChangeEvent<HTMLTextAreaElement>) => void;
  placeholder?: string;
  required?: boolean;
  rows?: number;
  cols?: number;
  disabled?: boolean;
  validationErrorMessage?: string;
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  [key: string]: any;
}

const Textarea: React.FC<TextareaProps> = ({
  label,
  id,
  name,
  value,
  onChange,
  placeholder = "",
  required = false,
  rows = 10,
  cols = 30,
  disabled = false,
  validationErrorMessage,
  ...rest
}) => {
  return (
    <div className="textarea-group">
      <label htmlFor={id}>{label}</label>
      <textarea
        id={id}
        name={name}
        value={value}
        onChange={onChange}
        placeholder={placeholder}
        required={required}
        rows={rows}
        cols={cols}
        disabled={disabled}
        className="textarea-field"
        {...rest}
      />
      {validationErrorMessage && <p>{validationErrorMessage}</p>}
    </div>
  );
};

export default Textarea;
